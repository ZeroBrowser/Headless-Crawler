using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;
using static ZeroBrowser.Crawler.Core.CrawlerDBContext;

namespace ZeroBrowser.Crawler.Core
{
    public class SQLiteRepository : IRepository
    {
        //private readonly CrawlerContext _crawlerContext;
        private ILogger<SQLiteRepository> _logger;

        public SQLiteRepository(ILogger<SQLiteRepository> logger)
        {
            //_crawlerContext = crawlerContext;
            _logger = logger;
        }

        public async Task AddPages(string parentUrl, List<string> pagesToCrawl)
        {
            using (var _crawlerContext = new CrawlerDBContext())
            {
                _logger.LogInformation($"CrawlerContext HashCode : {_crawlerContext.GetHashCode()}");

                var parent = !string.IsNullOrEmpty(parentUrl) ? await GetCrawledRecord<CrawledRecord>(cr => cr.HashedUrl == parentUrl.CreateMD5()) : null;

                var crawledRecords = pagesToCrawl.Select(p => createCrawledRecord(p));

                if (parent != null)
                {
                    foreach (var record in crawledRecords)
                    {
                        //add the record itself
                        await _crawlerContext.CrawledRecords.AddAsync(record);

                        //add relationship
                        await _crawlerContext.CrawledRecordRelations.AddAsync(new CrawledRecordRelation { Parent = parent, ParentId = parent.Id, Child = record, ChildId = record.Id });
                    }
                }

                await _crawlerContext.SaveChangesAsync();
            }
        }

        public async Task<bool> Exist(string url)
        {
            var record = await getCrawledRecord(url);

            return record != null;
        }

        public async Task UpdateHttpStatusCode(string url, HttpStatusCode statusCode)
        {
            var record = await getCrawledRecord(url);

            if (record == null)
                return;

            record.HttpStatusCode = statusCode;
            record.Updated = DateTime.UtcNow;

            using (var _crawlerContext = new CrawlerDBContext())
            {
                await _crawlerContext.SaveChangesAsync();
            }
        }


        public async Task<CrawledRecord> getCrawledRecord(string url)
        {
            var hashedUrl = url.CreateMD5();

            using (var _crawlerContext = new CrawlerDBContext())
            {
                var record = await _crawlerContext.CrawledRecords.FirstOrDefaultAsync(a => a.HashedUrl == hashedUrl);

                return record;
            }
        }

        public async Task<T> GetCrawledRecord<T>(Expression<Func<T, bool>> source) where T : class
        {
            using (var _crawlerContext = new CrawlerDBContext())
            {
                var record = await _crawlerContext.Set<T>().FirstOrDefaultAsync(source);

                return record;
            }
        }

        private CrawledRecord createCrawledRecord(string url)
        {
            url = url.ToLower();

            var crawledRecord = new CrawledRecord
            {
                Url = url,
                CrawlStatus = CrawlStatus.Pending,
                HashedUrl = url.CreateMD5(),
                HealthStatus = HealthStatus.Pending,
                HttpStatusCode = default,
                Inserted = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };

            return crawledRecord;
        }
    }
}

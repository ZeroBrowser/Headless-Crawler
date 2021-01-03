using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Core
{
    public class SQLiteRepository : IRepository
    {
        private readonly CrawlerContext _crawlerContext;

        public SQLiteRepository(CrawlerContext crawlerContext)
        {
            _crawlerContext = crawlerContext;
        }

        public async Task<bool> Exist(string url)
        {
            return false;
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Frontier
{
    public class Frontier : IFrontier
    {
        private readonly IUrlChannel _urlChannel;
        private readonly ILogger<Frontier> _logger;
        private FrontierState _frontierState;

        public Frontier(IUrlChannel urlProducer, ILogger<Frontier> logger, FrontierState frontierState)
        {
            _urlChannel = urlProducer;
            _logger = logger;
            _frontierState = frontierState;
        }

        /// <summary>
        /// It does what a Frontier does, add URL to queue for crawling
        /// </summary>
        /// <param name="crawlerContext">context</param>
        /// <returns>true if new URL found, false if existing</returns>
        public async Task<bool> Process(CrawlerContext crawlerContext)
        {
            var url = crawlerContext.CurrentUrl;

            if (url == null)
                return false;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                if (crawlerContext.IsSeed)
                {
                    _frontierState.Reset();
                    _frontierState.SeedUri = result;
                }

                if (_frontierState.ProcessedUrls.ContainsKey(url))
                {
                    //existing URL
                    _frontierState.ProcessedUrls[url].Total++;
                }
                else
                {
                    _logger.LogInformation($"New URL: {url}{Environment.NewLine}");
                    var parentPageInfo = new CrawledPageInfo { Url = crawlerContext.ParentUrl };
                    var crawledPageInfo = new CrawledPageInfo { ParentUrl = (url == crawlerContext.ParentUrl) ? null : crawlerContext.ParentUrl , Total = 1, Url = url };

                    //add to dict
                    _frontierState.ProcessedUrls.TryAdd(url, crawledPageInfo);

                    //add to tree as
                    var parent = _frontierState.CrawledTree.Find(parentPageInfo);
                    _frontierState.CrawledTree.Add(crawledPageInfo, parent);

                    await _urlChannel.Insert(crawlerContext);
                }


                return true;
            }

            return false;
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Frontier
{
    public class Frontier : IFrontier
    {        
        private readonly IUrlChannel _urlProducer;
        private readonly ILogger<Frontier> _logger;
        private FrontierState _frontierState;

        public Frontier(IUrlChannel urlProducer, ILogger<Frontier> logger, FrontierState frontierState)
        {
            _urlProducer = urlProducer;
            _logger = logger;
            _frontierState = frontierState;
        }

        public async Task Process(CrawlerContext crawlerContext)
        {
            if (crawlerContext.IsSeed)
                _frontierState.Reset();

            var url = crawlerContext.CurrentUrl;

            if (url == null)
                return;

            if (_frontierState.CrawledUrls.ContainsKey(url))
            {
                //stop

                _logger.LogInformation($"**** existing url: {url}{Environment.NewLine}");

                _frontierState.CrawledUrls[url]++;
            }
            else
            {
                //add to queue
                _logger.LogInformation($"**** new url: {url}{Environment.NewLine}");

                _frontierState.CrawledUrls.TryAdd(url, 1);
                await _urlProducer.Insert(crawlerContext);
            }
        }
    }
}

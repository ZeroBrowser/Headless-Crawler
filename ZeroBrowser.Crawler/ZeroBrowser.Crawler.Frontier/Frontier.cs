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
        //key is url and value is number of times this url is pass in to Frontier
        private readonly ConcurrentDictionary<string, int> _crawledUrls;
        private readonly ConcurrentQueue<string> _queue;
        private readonly IUrlChannel _urlProducer;
        private readonly ILogger<Frontier> _logger;

        public Frontier(IUrlChannel urlProducer, ILogger<Frontier> logger)
        {
            _crawledUrls = new ConcurrentDictionary<string, int>();
            _queue = new ConcurrentQueue<string>();
            _urlProducer = urlProducer;
            _logger = logger;
        }

        public async Task Process(CrawlerContext crawlerContext)
        {
            var url = crawlerContext.CurrentUrl;

            if (url == null)
                return;

            if (_crawledUrls.ContainsKey(url))
            {
                //stop

                _logger.LogInformation($"**** existing url: {url}{Environment.NewLine}");

                _crawledUrls[url]++;
            }
            else
            {
                //add to queue
                _logger.LogInformation($"**** new url: {url}{Environment.NewLine}");

                _crawledUrls.TryAdd(url, 1);
                //_queue.Enqueue(url);
                await _urlProducer.Insert(crawlerContext);
            }
        }
    }
}

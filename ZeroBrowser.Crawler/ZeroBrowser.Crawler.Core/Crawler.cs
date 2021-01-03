using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        private readonly ILogger<Crawler> _logger;
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private readonly IFrontier _frontier;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public Crawler(ILogger<Crawler> logger, IHeadlessBrowserService headlessBrowserService, IFrontier frontier, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;
            _frontier = frontier;
            _backgroundTaskQueue = backgroundTaskQueue;
        }


        public async Task Crawl(string url)
        {
            _logger.LogInformation($"Url : {url}");

            //1. lets get page information.
            var urls = await _headlessBrowserService.GetUrls(url);

            //2. list of pages to crawl
            var newUrls = await _frontier.Process(urls);

            //TODO: enforce limit
            foreach (var newUrl in newUrls)
                //TODO: do health check and save in DB using Repository

                _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    await Crawl(newUrl.ToString());
                });
        }
    }
}

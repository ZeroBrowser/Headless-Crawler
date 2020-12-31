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
        private readonly Parameters _parameters;
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private readonly IFrontier _frontier;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public Crawler(IHeadlessBrowserService headlessBrowserService, IFrontier frontier, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _headlessBrowserService = headlessBrowserService;
            _frontier = frontier;
            _backgroundTaskQueue = backgroundTaskQueue;
        }


        public async Task Crawl(string url)
        {
            //1. lets get page information.
            var urls = await _headlessBrowserService.GetUrls(url);

            var newUrls = await _frontier.Process(urls);

            foreach (var newUrl in newUrls)
            {
                _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    await Crawl(newUrl.ToString());
                });
            }
        }

    }
}

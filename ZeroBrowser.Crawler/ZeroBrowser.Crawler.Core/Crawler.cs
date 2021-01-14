using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;
using IFrontier = ZeroBrowser.Crawler.Core.Interfaces.IFrontier;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        private readonly ILogger<Crawler> _logger;
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private readonly IFrontier _frontier;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private static int jobIndex = 0;
        private static int totalPagesCrawled = 0;
        private static string seedHostName = string.Empty;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private readonly CrawlerOptions _crawlerOptions;
        private readonly IRepository _repository;
        private List<string> _blackList = new List<string> { "ws", "mailto" };

        public Crawler(ILogger<Crawler> logger,
                       IHeadlessBrowserService headlessBrowserService,
                       IFrontier frontier,
                       IBackgroundTaskQueue backgroundTaskQueue,
                       IOptions<CrawlerOptions> crawlerOptions,
                       IRepository repository,
                       IBackgroundUrlQueue backgroundUrlQueue)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;
            _frontier = frontier;
            _backgroundTaskQueue = backgroundTaskQueue;
            _crawlerOptions = crawlerOptions.Value;
            _repository = repository;
            _backgroundUrlQueue = backgroundUrlQueue;
        }


        public async Task Crawl(CrawlerContext crawlerContext)
        {
            var url = crawlerContext.CurrentUrl;

            //very first time lets populate the seed host name and cache it (static)
            //TODO need to set this once somehow.
            if (crawlerContext.IsSeed)
                seedHostName = new Uri(url).Host.Replace("www.", string.Empty);

            Task.Delay(_crawlerOptions.PolitenessDelay).Wait();

            var urls = await _headlessBrowserService.GetUrls(url, 0);

            if (urls == null)
                return;

            foreach (var newUrl in urls)
            {
                _logger.LogInformation($"***** new url found {url}.{Environment.NewLine}");

                if (!isUrlAllowed(newUrl.Url))
                    continue;

                _backgroundUrlQueue.QueueUrlItem(newUrl.Url);
            }
        }

        private bool isUrlAllowed(string url)
        {
            //lets not crawl if the site is outside seed url (main site)
            if (!url.Contains(seedHostName) || _blackList.Any(badKeyWord => url.Substring(0, badKeyWord.Length) == badKeyWord))
                return false;

            return true;
        }

        private void incrementCounter()
        {
            Interlocked.Increment(ref jobIndex);
            Interlocked.Increment(ref totalPagesCrawled);
        }

        private void resetCounter()
        {
            //lets reset to 0
            if (Volatile.Read(ref jobIndex) == _crawlerOptions.NumberOfParallelInstances)
            {
                Volatile.Write(ref jobIndex, 0);
            }
        }
    }
}

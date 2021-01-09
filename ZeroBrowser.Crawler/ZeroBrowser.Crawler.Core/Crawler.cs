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

        private readonly CrawlerOptions _crawlerOptions;
        private readonly IRepository _repository;

        public Crawler(ILogger<Crawler> logger,
                       IHeadlessBrowserService headlessBrowserService,
                       IFrontier frontier,
                       IBackgroundTaskQueue backgroundTaskQueue,
                       IOptions<CrawlerOptions> crawlerOptions,
                       IRepository repository)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;
            _frontier = frontier;
            _backgroundTaskQueue = backgroundTaskQueue;
            _crawlerOptions = crawlerOptions.Value;
            _repository = repository;
        }


        public async Task Crawl(string url)
        {
            //very first time lets populate the seed host name and cache it (static)
            if (seedHostName == string.Empty)
            {
                seedHostName = new Uri(url).Host.Replace("www.", string.Empty);
                await _repository.AddPages(null, new List<string> { url });
            }

            _logger.LogInformation($"Url : {url}");

            //1. lets get the page information
            var urls = await _headlessBrowserService.GetUrls(url, jobIndex);

            //2. list of all pages to crawl
            var newUrls = await _frontier.Process(url, urls);

            foreach (var newUrl in newUrls)
            {
                //lets not crawl if the site is outside seed url (main site)
                if (!newUrl.Contains(seedHostName))
                    continue;

                //do health check and save in DB using Repository
                var httpStatusCode = await _headlessBrowserService.HealthCheck(url, jobIndex);
                await _repository.UpdateHttpStatusCode(url, httpStatusCode);

                //if total pages crawled reaches max number of pages allowed Enforce limit
                if (_crawlerOptions.MaxNumOfPagesAllowedToBeCrawled == Volatile.Read(ref totalPagesCrawled))
                    break;

                Task.Delay(1000).Wait();

                incrementCounter();

                _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    await Crawl(newUrl.ToString());
                });

                resetCounter();
            }
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

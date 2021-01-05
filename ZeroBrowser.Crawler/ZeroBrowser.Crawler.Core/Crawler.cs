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
            _logger.LogInformation($"Url : {url}");

            //1. lets get page information.
            var urls = await _headlessBrowserService.GetUrls(url, jobIndex);

            //2. list of pages to crawl
            var newUrls = await _frontier.Process(urls);

            foreach (var newUrl in newUrls)
            {
                //do health check and save in DB using Repository
                var httpStatusCode = await _headlessBrowserService.HealthCheck(url, jobIndex);
                await _repository.UpdateHttpStatusCode(url, httpStatusCode);

                //Enforce limit
                if (_crawlerOptions.MaxNumOfPagesToCrawl == Volatile.Read(ref totalPagesCrawled))
                    break;

                _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    Interlocked.Increment(ref jobIndex);
                    Interlocked.Increment(ref totalPagesCrawled);

                    await Crawl(newUrl.ToString());

                    //lets reset to 0
                    if (Volatile.Read(ref jobIndex) == _crawlerOptions.NumberOfParallelInstances)
                    {
                        Volatile.Write(ref jobIndex, 0);
                    }
                });
            }
        }
    }
}

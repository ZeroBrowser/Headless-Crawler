using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        //private static string 

        private int _maxNumOfParallelOperations = 10;
        private int _executorsCount = 2;
        private readonly IConfiguration _configuration;

        public Crawler(ILogger<Crawler> logger,
                       IHeadlessBrowserService headlessBrowserService,
                       IFrontier frontier,
                       IBackgroundTaskQueue backgroundTaskQueue,
                       IConfiguration configuration)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;            
            _frontier = frontier;
            _backgroundTaskQueue = backgroundTaskQueue;
            _configuration = configuration;
        }

        private void initFromConfiguration()
        {
            if (ushort.TryParse(_configuration["App:MaxNumOfParallelOperations"], out var maxValue))
                _maxNumOfParallelOperations = maxValue;

            //lets cap it to _maxNumOfParallelOperations
            if (ushort.TryParse(_configuration["App:NumOfParallelOperations"], out var value))
                _executorsCount = value > _maxNumOfParallelOperations ? _maxNumOfParallelOperations : value;
        }

        public async Task Crawl(string url)
        {
            _logger.LogInformation($"Url : {url}");

            //1. lets get page information.
            var urls = await _headlessBrowserService.GetUrls(url, jobIndex);

            //2. list of pages to crawl
            var newUrls = await _frontier.Process(urls);

            //TODO: enforce limit
            foreach (var newUrl in newUrls)
                //TODO: do health check and save in DB using Repository

                _backgroundTaskQueue.QueueBackgroundWorkItem(async token =>
                {
                    Interlocked.Increment(ref jobIndex);
                    await Crawl(newUrl.ToString());

                    //lets reset to 0
                    if (Volatile.Read(ref jobIndex) == _executorsCount)
                    {
                        Volatile.Write(ref jobIndex, 0);
                    }
                });
        }
    }
}

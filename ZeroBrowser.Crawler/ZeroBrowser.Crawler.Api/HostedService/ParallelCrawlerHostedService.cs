using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZeroBrowser.Crawler.Common.CustomValidations;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Api.HostedService
{
    public class ParallelCrawlerHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private int _maxNumOfParallelOperations = 10;
        private int _executorsCount = 2;
        private IUrlChannel _urlChannel { get; }
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private static string seedHostName = string.Empty;
        private CrawlerOptions _crawlerOptions;

        public ParallelCrawlerHostedService(IUrlChannel urlChannel,
                                            ILoggerFactory loggerFactory,
                                            IOptions<CrawlerOptions> crawlerOptions,
                                            IHeadlessBrowserService headlessBrowserService,
                                            IBackgroundUrlQueue backgroundUrlQueue)
        {
            _crawlerOptions = crawlerOptions.Value;
            _urlChannel = urlChannel;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
            _headlessBrowserService = headlessBrowserService;
            _backgroundUrlQueue = backgroundUrlQueue;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"***** entered consumer.{Environment.NewLine}");

            for (var i = 0; i < _executorsCount; i++)
            {
                var executorTask = new Task(
                    async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            await foreach (var crawlerContext in await _urlChannel.Read())
                            {
                                var url = crawlerContext.CurrentUrl;

                                //very first time lets populate the seed host name and cache it (static)
                                //TODO need to set this once somehow.
                                if (seedHostName == string.Empty)
                                    seedHostName = new Uri(url).Host.Replace("www.", string.Empty);

                                var urls = await _headlessBrowserService.GetUrls(url, 0);

                                if (urls == null)
                                    continue;

                                foreach (var newUrl in urls)
                                {
                                    _logger.LogInformation($"***** new url found {url}.{Environment.NewLine}");

                                    //lets not crawl if the site is outside seed url (main site)
                                    if (!newUrl.Url.Contains(seedHostName) || new HashSet<string> { "ws", "ws", "mailto" }.Contains(newUrl.Url))
                                        continue;

                                    _backgroundUrlQueue.QueueUrlItem(newUrl.Url);
                                }
                            }
                        }
                    }, cancellationToken);

                executorTask.Start();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}

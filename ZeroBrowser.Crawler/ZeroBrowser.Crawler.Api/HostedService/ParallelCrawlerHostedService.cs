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
        private readonly int _executorsCount;
        private readonly Task[] _executors;
        private IUrlChannel _urlChannel;
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private static string seedHostName = string.Empty;
        private CrawlerOptions _crawlerOptions;
        private CancellationTokenSource _tokenSource;
        private List<string> _blackList = new List<string> { "ws", "wss", "mailto" };

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
            _executorsCount = _crawlerOptions.NumberOfParallelInstances;
            _executors = new Task[_executorsCount];
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"***** entered consumer.{Environment.NewLine}");

            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            for (var i = 0; i < _executorsCount; i++)
            {
                var executorTask = new Task(
                    async () =>
                    {

                        await foreach (var crawlerContext in await _urlChannel.Read())
                        {
                            var url = crawlerContext.CurrentUrl;

                            //very first time lets populate the seed host name and cache it (static)
                            //TODO need to set this once somehow.
                            if (crawlerContext.IsSeed)
                                seedHostName = new Uri(url).Host.Replace("www.", string.Empty);

                            var urls = await _headlessBrowserService.GetUrls(url, 0);

                            if (urls == null)
                                continue;

                            foreach (var newUrl in urls)
                            {
                                _logger.LogInformation($"***** new url found {url}.{Environment.NewLine}");

                                if (!isUrlAllowed(newUrl.Url))
                                    continue;

                                _backgroundUrlQueue.QueueUrlItem(newUrl.Url);
                            }
                        }

                    }, _tokenSource.Token);

                _executors[i] = executorTask;
                executorTask.Start();
            }
        }

        private bool isUrlAllowed(string url)
        {
            //lets not crawl if the site is outside seed url (main site)
            if (!url.Contains(seedHostName) || _blackList.Any(badKeyWord => url.Substring(0, badKeyWord.Length - 1) == badKeyWord))
                return false;

            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel(); // send the cancellation signal

            if (_executors != null)
            {
                // wait for _executors completion
                Task.WaitAll(_executors, cancellationToken);
            }

            return Task.CompletedTask;
        }
    }
}

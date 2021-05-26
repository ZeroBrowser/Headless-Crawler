using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private CrawlerOptions _crawlerOptions;
        private CancellationTokenSource _tokenSource;
        private readonly ICrawler _crawler;

        public ParallelCrawlerHostedService(IUrlChannel urlChannel,
                                            ILoggerFactory loggerFactory,
                                            IOptions<CrawlerOptions> crawlerOptions,
                                            ICrawler crawler)
        {
            _crawlerOptions = crawlerOptions.Value;
            _urlChannel = urlChannel;
            _logger = loggerFactory.CreateLogger<ParallelCrawlerHostedService>();
            _executorsCount = _crawlerOptions.NumberOfParallelInstances;
            _executors = new Task[_executorsCount];
            _crawler = crawler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var crawlerIndex = 0;
            for (var i = 0; i < _executorsCount; i++)
            {
                var executorTask = new Task(
                    async () =>
                    {
                        await foreach (var crawlerContext in await _urlChannel.Read())
                        {
                            if (crawlerIndex > _executorsCount)
                                crawlerIndex = 0;

                            crawlerContext.CurrentCrawlerIndex = crawlerIndex;

                            await _crawler.Crawl(crawlerContext);
                            crawlerIndex++;
                        }

                    }, _tokenSource.Token);

                _executors[i] = executorTask;
                executorTask.Start();
            }
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

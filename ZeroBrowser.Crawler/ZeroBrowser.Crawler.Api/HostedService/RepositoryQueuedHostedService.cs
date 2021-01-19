using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Core;
using static ZeroBrowser.Crawler.Core.CrawlerDBContext;

namespace ZeroBrowser.Crawler.Api.HostedService
{
    public class RepositoryQueuedHostedService : BackgroundService
    {
        private readonly ILogger<RepositoryQueuedHostedService> _logger;
        private readonly CrawlerDBContext _crawlerDBContext;

        public RepositoryQueuedHostedService(IRepositoryQueue repositoryQueue, ILogger<RepositoryQueuedHostedService> logger, CrawlerDBContext crawlerDBContext)
        {
            RepositoryQueue = repositoryQueue;
            _logger = logger;
            _crawlerDBContext = crawlerDBContext;
        }

        public IRepositoryQueue RepositoryQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"*** ExecuteAsync.{Environment.NewLine}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await RepositoryQueue.DequeueAsync();

                _logger.LogInformation($"*** Dequeued : {context.CurrentUrl}{Environment.NewLine}");

                try
                {
                    //Save in repo
                    await _crawlerDBContext.CrawledRecords.AddAsync(new CrawledRecord { });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred processing url : {url}.", context.CurrentUrl);
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}

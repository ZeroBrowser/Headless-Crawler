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
    public class RepositoryQueuedHostedService : BackgroundService
    {
        private readonly ILogger<RepositoryQueuedHostedService> _logger;
        private readonly IRepository _repository;
        private readonly CrawlerOptions _crawlerOptions;

        public RepositoryQueuedHostedService(IRepositoryQueue repositoryQueue,
                                             ILogger<RepositoryQueuedHostedService> logger,
                                             IRepository repository,
                                             IOptions<CrawlerOptions> crawlerOptions)
        {
            RepositoryQueue = repositoryQueue;
            _logger = logger;
            _repository = repository;
            _crawlerOptions = crawlerOptions.Value;
        }

        public IRepositoryQueue RepositoryQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_crawlerOptions.DisableDB)
                return;

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await RepositoryQueue.DequeueAsync();

                _logger.LogInformation($"*** Dequeued : {context.CurrentUrl}{Environment.NewLine}");

                try
                {
                    //Save in repo
                    await _repository.AddPage(context);
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

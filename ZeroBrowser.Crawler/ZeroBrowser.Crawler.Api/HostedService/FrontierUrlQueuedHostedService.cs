using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Api.HostedService
{
    public class FrontierUrlQueuedHostedService : BackgroundService
    {
        private readonly ILogger<FrontierUrlQueuedHostedService> _logger;
        private readonly IFrontier _frontier;

        public FrontierUrlQueuedHostedService(IBackgroundUrlQueue urlQueue, ILogger<FrontierUrlQueuedHostedService> logger, IFrontier frontier)
        {
            UrlQueue = urlQueue;
            _logger = logger;
            _frontier = frontier;
        }

        public IBackgroundUrlQueue UrlQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"*** ExecuteAsync.{Environment.NewLine}");

            while (!stoppingToken.IsCancellationRequested)
            {
                var context = await UrlQueue.DequeueAsync();

                _logger.LogInformation($"*** Dequeued : {context.CurrentUrl}{Environment.NewLine}");

                try
                {
                    await _frontier.Process(context);
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

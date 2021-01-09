using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Api.HostedService
{
    public class ParallelCrawlerHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private int _maxNumOfParallelOperations = 10;
        private int _executorsCount = 2;
        private readonly Task[] _executors;        
        private CancellationTokenSource _tokenSource;
        private IUrlChannel _urlChannel { get; }
        private IConfiguration _configuration;

        public ParallelCrawlerHostedService(IUrlChannel urlChannel, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _urlChannel = urlChannel;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
            _configuration = configuration;

            initFromConfiguration();

            _executors = new Task[_executorsCount];
        }

        private void initFromConfiguration()
        {
            if (ushort.TryParse(_configuration["App:MaxNumOfParallelOperations"], out var maxValue))
                _maxNumOfParallelOperations = maxValue;

            //lets cap it to _maxNumOfParallelOperations
            if (ushort.TryParse(_configuration["App:NumOfParallelOperations"], out var value))
                _executorsCount = value > _maxNumOfParallelOperations ? _maxNumOfParallelOperations : value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Wait while channel is not empty and still not completed
            await foreach (var item in _urlChannel.Read())
            {
                _ = Task.Factory.StartNew(async () =>
                  {
                    //do something inside here
                }, TaskCreationOptions.LongRunning);                
            }

            //return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {

            return Task.CompletedTask;
        }
    }
}

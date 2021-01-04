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
    public class ParallelQueuedHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private int _maxNumOfParallelOperations = 10;
        private int _executorsCount = 2;
        private readonly Task[] _executors;        
        private CancellationTokenSource _tokenSource;
        public IBackgroundTaskQueue TaskQueue { get; }
        private IConfiguration _configuration;

        public ParallelQueuedHostedService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            TaskQueue = taskQueue;
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            for (var i = 0; i < _executorsCount; i++)
            {
                var executorTask = new Task(
                    async () =>
                    {
                        while (!cancellationToken.IsCancellationRequested)
                        {
#if DEBUG
                            _logger.LogInformation("Waiting background task...");
#endif
                            var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                            try
                            {
#if DEBUG
                                _logger.LogInformation("Got background task, executing...");
#endif
                                await workItem(cancellationToken);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex,
                                    "Error occurred executing {WorkItem}.", nameof(workItem)
                                );
                            }
                        }
                    }, _tokenSource.Token);

                _executors[i] = executorTask;
                executorTask.Start();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");
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

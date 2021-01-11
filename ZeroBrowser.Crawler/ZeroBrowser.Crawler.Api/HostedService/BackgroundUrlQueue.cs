using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Api.HostedService
{
    public class BackgroundUrlQueue : IBackgroundUrlQueue
    {
        private ConcurrentQueue<string> _workItems = new ConcurrentQueue<string>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly ILogger<BackgroundUrlQueue> _logger;

        public BackgroundUrlQueue(ILogger<BackgroundUrlQueue> logger)
        {
            _logger = logger;
        }

        public void QueueUrlItem(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            _logger.LogInformation($"** Enqueue {url}{Environment.NewLine}");

            _workItems.Enqueue(url);
            _signal.Release();
        }

        public async Task<string> DequeueAsync()
        {
            //if (_signal.CurrentCount == 0)
            //    return null;

            await _signal.WaitAsync();
            _workItems.TryDequeue(out var url);

            _logger.LogInformation($"** Dequeue {url}{Environment.NewLine}");

            return url;
        }
    }
}

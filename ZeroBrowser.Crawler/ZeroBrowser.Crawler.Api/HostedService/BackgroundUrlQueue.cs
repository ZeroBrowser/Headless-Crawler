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

        public void QueueUrlItem(string url)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            _workItems.Enqueue(url);
            _signal.Release();
        }

        public async Task<string> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}

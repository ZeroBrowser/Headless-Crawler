using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Queues
{
    public class RepositoryQueue : IRepositoryQueue
    {
        private ConcurrentQueue<CrawlerContext> _crawlerContextQueue = new ConcurrentQueue<CrawlerContext>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly ILogger<RepositoryQueue> _logger;
        private CrawlerContext _crawlerContext;

        public RepositoryQueue(ILogger<RepositoryQueue> logger)
        {
            _logger = logger;
        }

        public void QueueUrlItem(CrawlerContext crawlerContext)
        {
            if (crawlerContext == null)
            {
                throw new ArgumentNullException(nameof(crawlerContext));
            }

            _crawlerContextQueue.Enqueue(_crawlerContext);
            _signal.Release();
        }

        public async Task<CrawlerContext> DequeueAsync()
        {
            await _signal.WaitAsync();

            _crawlerContextQueue.TryDequeue(out var crawlerContext);

            _logger.LogInformation($"** Dequeue {crawlerContext.CurrentUrl}{Environment.NewLine}");

            return crawlerContext;
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Queues
{
    public class BackgroundUrlQueue : IBackgroundUrlQueue
    {
        private ConcurrentQueue<CrawlerContext> _workItems = new ConcurrentQueue<CrawlerContext>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly ILogger<BackgroundUrlQueue> _logger;
        private CrawlerContext _crawlerContext;

        public BackgroundUrlQueue(ILogger<BackgroundUrlQueue> logger)
        {
            _logger = logger;
        }

        public void EnqueteUrlItem(string url, bool isSeed = false, string parentUrl = null)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            _crawlerContext = new CrawlerContext() { CurrentUrl = url, ParentUrl = parentUrl, IsSeed = isSeed };

            _workItems.Enqueue(_crawlerContext);
            _signal.Release();
        }

        public async Task<CrawlerContext> DequeueAsync()
        {
            await _signal.WaitAsync();

            _workItems.TryDequeue(out var crawlerContext);

            return crawlerContext;
        }
    }
}

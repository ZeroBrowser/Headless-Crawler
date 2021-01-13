﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Api.HostedService
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

        public void QueueUrlItem(string url, bool isSeed = false)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (isSeed)
            {
                _crawlerContext = new CrawlerContext() { CurrentUrl = url, ParentUrl = url };
            }

            _logger.LogInformation($"** Enqueue {url}{Environment.NewLine}");

            _workItems.Enqueue(_crawlerContext);
            _signal.Release();
        }

        public async Task<CrawlerContext> DequeueAsync()
        {
            await _signal.WaitAsync();

            _workItems.TryDequeue(out var crawlerContext);

            _logger.LogInformation($"** Dequeue {crawlerContext.CurrentUrl}{Environment.NewLine}");

            return crawlerContext;
        }
    }
}

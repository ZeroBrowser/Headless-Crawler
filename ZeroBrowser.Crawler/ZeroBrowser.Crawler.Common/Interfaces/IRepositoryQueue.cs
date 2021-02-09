using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IBackgroundUrlQueue
    {
        void QueueUrlItem(string url, bool isSeed = false);

        Task<CrawlerContext> DequeueAsync();
    }
}

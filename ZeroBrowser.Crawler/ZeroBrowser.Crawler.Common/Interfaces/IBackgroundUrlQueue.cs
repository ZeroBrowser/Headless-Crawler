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
        void EnqueteUrlItem(string url, bool isSeed = false, string parentUrl = null);

        Task<CrawlerContext> DequeueAsync();
    }
}

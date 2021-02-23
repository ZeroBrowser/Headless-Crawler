using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IRepositoryQueue
    {

        void QueueUrlItem(CrawlerContext crawlerContext);

        Task<CrawlerContext> DequeueAsync();
    }
}

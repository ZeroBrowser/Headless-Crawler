using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Models;

namespace ZeroBrowser.Crawler.Core.Interfaces
{
    public interface ICrawler
    {
        Task<IAsyncEnumerable<WebPage>> Crawl();
    }
}

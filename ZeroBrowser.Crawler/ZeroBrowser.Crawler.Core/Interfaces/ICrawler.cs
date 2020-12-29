using System;
using System.Collections.Generic;
using System.Text;
using ZeroBrowser.Crawler.Core.Models;

namespace ZeroBrowser.Crawler.Core.Interfaces
{
    public interface ICrawler
    {
        IAsyncEnumerable<WebPage> Crawl(string[] seedUrls);
    }
}

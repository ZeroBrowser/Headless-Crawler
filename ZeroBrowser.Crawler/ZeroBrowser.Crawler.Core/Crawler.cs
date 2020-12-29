using System;
using System.Collections.Generic;
using ZeroBrowser.Crawler.Core.Interfaces;
using ZeroBrowser.Crawler.Core.Models;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        public Crawler(string[] seedUrls)
        {

        }


        public async IAsyncEnumerable<WebPage> Crawl(string[] seedUrls)
        {

            throw new NotImplementedException();
        }
    }
}

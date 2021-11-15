using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Common.Tree;

namespace ZeroBrowser.Crawler.Frontier
{
    public class FrontierState
    {
        public Uri SeedUri { get; set; }

        //key is URL and value is number of times this URL is pass in to Frontier
        public ConcurrentDictionary<string, CrawledPageInfo> ProcessedUrls { get; private set; }

        public ICrawledTree<CrawledPageInfo> CrawledTree { get; private set; }

        public FrontierState()
        {
            ProcessedUrls = new ConcurrentDictionary<string, CrawledPageInfo>();
            CrawledTree = new CrawledTree<CrawledPageInfo>();
        }

        internal void Reset()
        {
            if (ProcessedUrls.Count > 0)
            {
                ProcessedUrls = new ConcurrentDictionary<string, CrawledPageInfo>();
            }

            if (CrawledTree.Root != null)
            {

                CrawledTree = new CrawledTree<CrawledPageInfo>();
            }
        }
    }
}

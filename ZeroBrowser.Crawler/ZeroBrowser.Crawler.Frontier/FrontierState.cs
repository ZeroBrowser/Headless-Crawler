﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Frontier
{
    public class FrontierState
    {
        public Uri SeedUri { get; set; }

        //key is URL and value is number of times this URL is pass in to Frontier
        public ConcurrentDictionary<string, int> CrawledUrls { get; private set; }

        public FrontierState()
        {
            CrawledUrls = new ConcurrentDictionary<string, int>();
        }

        internal void Reset()
        {
            if (CrawledUrls.Count > 0)
            {
                CrawledUrls = new ConcurrentDictionary<string, int>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Common.Models
{
    public class CrawlerContext
    {
        public string CurrentUrl { get; set; }

        public string ParentUrl { get; set; }

        public bool IsSeed { get { return CurrentUrl == ParentUrl; } }
    }
}

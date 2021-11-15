using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Common.Models
{
    public interface ICrawledPageInfo
    {
        string Url { get; set; }

    }

    public class CrawledPageInfo : ICrawledPageInfo
    {
        public int Total { get; set; }

        public string Url { get; set; }

        public string ParentUrl { get; set; }
    }
}

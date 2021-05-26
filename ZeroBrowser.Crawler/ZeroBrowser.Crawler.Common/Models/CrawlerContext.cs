using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroBrowser.Crawler.Common.CustomValidations;

namespace ZeroBrowser.Crawler.Common.Models
{
    public class CrawlerContext
    {
        public string CurrentUrl { get; set; }

        public string ParentUrl { get; set; }

        public bool IsSeed { get; set; }

        public int CurrentCrawlerIndex { get; set; }
    }
}

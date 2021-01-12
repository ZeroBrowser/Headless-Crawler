using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Common.Models
{
    public class CrawlerOptions
    {
        public const string Section = "App";

        public string HeadlessBrowserUrl { get; set; }

        public bool LazyInitBrowserPage { get; set; }

        private ushort _numberOfParallelInstances;
        public ushort NumberOfParallelInstances
        {
            get
            {
                //lets cap it to _maxNumOfParallelOperations
                return (_numberOfParallelInstances > MaxNumberOfParallelInstances) ? MaxNumberOfParallelInstances : _numberOfParallelInstances;
            }
            set { _numberOfParallelInstances = value; }
        }

        public ushort MaxNumberOfParallelInstances { get; set; }

        public string JQueryUrl { get; set; }

        public ushort MaxNumOfPagesAllowedToBeCrawled { get; set; }

        public string SeedUrl { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IUrlChannel
    {
        Task Insert(CrawlerContext crawlerContext);

        Task<IAsyncEnumerable<CrawlerContext>> Read();
    }
}

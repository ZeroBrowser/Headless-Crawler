using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Core
{
    public class CrawlerContext : DbContext
    {
        public CrawlerContext(DbContextOptions<CrawlerContext> options) : base(options)
        {

        }

        public DbSet<CrawlerState> CrawlerStates { get; set; }

        public enum CrawlerStatus
        {
            NotCrawled,
            Crawled
        }

        public enum HealthStatus
        {
            Healthy,
            NotHealthy
        }


        public class CrawlerState
        {
            public int CrawlerStateId { get; set; }

            public string Url { get; set; }

            public CrawlerStatus Status { get; set; }

            public HealthStatus HealthStatus { get; set; }
        }
    }
}

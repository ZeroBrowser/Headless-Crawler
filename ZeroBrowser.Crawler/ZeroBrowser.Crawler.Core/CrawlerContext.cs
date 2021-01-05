using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ZeroBrowser.Crawler.Core
{
    public class CrawlerContext : DbContext
    {
        public CrawlerContext(DbContextOptions<CrawlerContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<CrawledRecord>().HasIndex(b => b.HashedUrl);
        }

        public DbSet<CrawledRecord> CrawledRecords { get; set; }

        public enum CrawlStatus
        {
            Pending,
            Processing,
            Processed
        }

        public enum HealthStatus
        {
            Pending,
            Healthy,
            UnHealthy
        }


        public class CrawledRecord
        {
            public Guid Id { get; set; }

            public string Url { get; set; }

            public string HashedUrl { get; set; }

            public CrawlStatus CrawlStatus { get; set; }

            public HealthStatus HealthStatus { get; set; }

            public HttpStatusCode? HttpStatusCode { get; set; }

            public DateTime Inserted { get; set; }

            public DateTime Updated { get; set; }
        }
    }
}

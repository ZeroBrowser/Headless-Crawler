using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;

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
            
            modelBuilder.Entity<CrawledRecordRelation>()
                    .HasKey(x => new { x.ParentId, x.ChildId });

            modelBuilder.Entity<CrawledRecordRelation>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.CrawledRecords)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CrawledRecordRelation>()
                .HasOne(x => x.Child)
                .WithMany(x => x.ParentCrawledRecord)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<CrawledRecord> CrawledRecords { get; set; }
        public DbSet<CrawledRecordRelation> CrawledRecordRelations { get; set; }

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

            public virtual ICollection<CrawledRecordRelation> CrawledRecords { get; set; }
            public virtual ICollection<CrawledRecordRelation> ParentCrawledRecord { get; set; }
        }

        public class CrawledRecordRelation
        {
            public Guid ParentId { get; set; }
            public CrawledRecord Parent { get; set; }

            public Guid ChildId { get; set; }
            public CrawledRecord Child { get; set; }
        }
    }
}

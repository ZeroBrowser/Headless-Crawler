﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZeroBrowser.Crawler.Core;

namespace ZeroBrowser.Crawler.Core.Migrations
{
    [DbContext(typeof(CrawlerContext))]
    partial class CrawlerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("ZeroBrowser.Crawler.Core.CrawlerContext+CrawledRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("CrawlStatus")
                        .HasColumnType("INTEGER");

                    b.Property<string>("HashedUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("HealthStatus")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("HttpStatusCode")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Inserted")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Updated")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("HashedUrl");

                    b.HasIndex("Id");

                    b.ToTable("CrawledRecords");
                });
#pragma warning restore 612, 618
        }
    }
}

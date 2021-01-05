using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroBrowser.Crawler.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CrawledRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
                    HashedUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CrawlStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    HealthStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    HttpStatusCode = table.Column<int>(type: "INTEGER", nullable: true),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawledRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrawledRecords_HashedUrl",
                table: "CrawledRecords",
                column: "HashedUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrawledRecords");
        }
    }
}

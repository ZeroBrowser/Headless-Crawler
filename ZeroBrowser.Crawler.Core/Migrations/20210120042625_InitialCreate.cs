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
                    HealthStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    HttpStatusCode = table.Column<int>(type: "INTEGER", nullable: true),
                    Inserted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Updated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawledRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrawledRecordRelations",
                columns: table => new
                {
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ChildId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrawledRecordRelations", x => new { x.ParentId, x.ChildId });
                    table.ForeignKey(
                        name: "FK_CrawledRecordRelations_CrawledRecords_ChildId",
                        column: x => x.ChildId,
                        principalTable: "CrawledRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CrawledRecordRelations_CrawledRecords_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CrawledRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CrawledRecordRelations_ChildId",
                table: "CrawledRecordRelations",
                column: "ChildId");

            migrationBuilder.CreateIndex(
                name: "IX_CrawledRecords_HashedUrl",
                table: "CrawledRecords",
                column: "HashedUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CrawledRecordRelations");

            migrationBuilder.DropTable(
                name: "CrawledRecords");
        }
    }
}

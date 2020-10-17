using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace BingWallpaper.Data.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ImageInfos",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    Copyright = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageInfos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageInfos_Date",
                schema: "dbo",
                table: "ImageInfos",
                column: "Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageInfos",
                schema: "dbo");
        }
    }
}

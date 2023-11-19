using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Images",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false).Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Headline = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                Url = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                Copyright = table.Column<string>(type: "TEXT", maxLength: 2048, nullable: false),
                CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Images", x => x.Id));

        migrationBuilder.CreateTable(
            name: "Settings",
            columns: table => new
            {
                Key = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Value = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Settings", x => x.Key));

        migrationBuilder.CreateIndex(name: "IX_Images_Url", table: "Images", column: "Url");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Images");

        migrationBuilder.DropTable(name: "Settings");
    }
}

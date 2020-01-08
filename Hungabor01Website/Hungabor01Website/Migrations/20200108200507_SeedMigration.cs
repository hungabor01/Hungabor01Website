using Microsoft.EntityFrameworkCore.Migrations;

namespace Hungabor01Website.Migrations
{
    public partial class SeedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TestTable",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Gábor" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TestTable",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}

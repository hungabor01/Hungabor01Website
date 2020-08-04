using Microsoft.EntityFrameworkCore.Migrations;

namespace Hungabor01Website.Migrations
{
    public partial class RenameAttachmentTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "AccountHistories");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "AccountHistories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "AccountHistories");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "AccountHistories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}

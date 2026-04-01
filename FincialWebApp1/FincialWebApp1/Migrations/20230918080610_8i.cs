using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincialWebApp1.Migrations
{
    public partial class _8i : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheId",
                table: "Trackers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TheId",
                table: "Trackers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

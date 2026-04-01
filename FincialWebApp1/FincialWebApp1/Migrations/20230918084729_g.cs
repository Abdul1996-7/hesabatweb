using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincialWebApp1.Migrations
{
    public partial class g : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TheId",
                table: "Trackers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheId",
                table: "Trackers");
        }
    }
}

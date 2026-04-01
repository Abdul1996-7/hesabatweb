using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincialWebApp1.Migrations
{
    public partial class lk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TheId",
                table: "MainClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TheId",
                table: "MainClass",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

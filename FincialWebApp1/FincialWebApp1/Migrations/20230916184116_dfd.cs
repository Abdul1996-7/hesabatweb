using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FincialWebApp1.Migrations
{
    public partial class dfd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankList",
                columns: table => new
                {
                    BankListId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankList", x => x.BankListId);
                });

            migrationBuilder.CreateTable(
                name: "MainClass",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheId = table.Column<int>(type: "int", nullable: false),
                    WhoEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfEdit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WhoDelet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfDelet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CTnumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CTdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    dateofTravel = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LeftAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreparationAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfAddingbyCutterEmployer = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepotName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Import = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Stamps = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransportationLoads = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BankListId = table.Column<int>(type: "int", nullable: true),
                    A = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    B = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    C = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ischecked = table.Column<bool>(type: "bit", nullable: false),
                    cheackNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cheackDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    checkDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainClass_BankList_BankListId",
                        column: x => x.BankListId,
                        principalTable: "BankList",
                        principalColumn: "BankListId");
                });

            migrationBuilder.CreateTable(
                name: "Trackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TheId = table.Column<int>(type: "int", nullable: false),
                    WhoEdit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfEdit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WhoDelet = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfDelet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CTnumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CTdate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreparationAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfAddingbyCutterEmployer = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepotName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Import = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Stamps = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransportationLoads = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FormCuttingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CutAmountCost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateofDEpot = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BankListId = table.Column<int>(type: "int", nullable: false),
                    A = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    B = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cheackNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cheackDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    C = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    checkDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ischecked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trackers_BankList_BankListId",
                        column: x => x.BankListId,
                        principalTable: "BankList",
                        principalColumn: "BankListId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainClass_BankListId",
                table: "MainClass",
                column: "BankListId");

            migrationBuilder.CreateIndex(
                name: "IX_Trackers_BankListId",
                table: "Trackers",
                column: "BankListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainClass");

            migrationBuilder.DropTable(
                name: "Trackers");

            migrationBuilder.DropTable(
                name: "BankList");
        }
    }
}

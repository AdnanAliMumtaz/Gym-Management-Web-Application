using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations.ManagingDb
{
    public partial class AddMembersAndTransactionFees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberDateJoined = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MemberDateLeft = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.MemberID);
                });

            migrationBuilder.CreateTable(
                name: "TransactionFees",
                columns: table => new
                {
                    TransactionFeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePaid = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MemberID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionFees", x => x.TransactionFeeID);
                    table.ForeignKey(
                        name: "FK_TransactionFees_Members_MemberID",
                        column: x => x.MemberID,
                        principalTable: "Members",
                        principalColumn: "MemberID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionFees_MemberID",
                table: "TransactionFees",
                column: "MemberID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionFees");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}

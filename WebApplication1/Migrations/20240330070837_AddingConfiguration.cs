using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class AddingConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEmployees_Employees_EmployeeID",
                table: "ClassEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassMembers_Member_MemberID",
                table: "ClassMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEmployees_Employees_EmployeeID",
                table: "ClassEmployees",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassMembers_Member_MemberID",
                table: "ClassMembers",
                column: "MemberID",
                principalTable: "Member",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassEmployees_Employees_EmployeeID",
                table: "ClassEmployees");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassMembers_Member_MemberID",
                table: "ClassMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassEmployees_Employees_EmployeeID",
                table: "ClassEmployees",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassMembers_Member_MemberID",
                table: "ClassMembers",
                column: "MemberID",
                principalTable: "Member",
                principalColumn: "MemberID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

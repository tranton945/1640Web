using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _50 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserDepartment");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ApplicationUserId",
                table: "Departments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ApplicationUserId",
                table: "Departments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ApplicationUserId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ApplicationUserId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Departments");

            migrationBuilder.CreateTable(
                name: "ApplicationUserDepartment",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserDepartment", x => new { x.DepartmentsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserDepartment_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserDepartment_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserDepartment_UsersId",
                table: "ApplicationUserDepartment",
                column: "UsersId");
        }
    }
}

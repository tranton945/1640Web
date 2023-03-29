using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _100 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Submissions_SubmissionId",
                table: "Ideas");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "Ideas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Ideas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CatogoryId",
                table: "Ideas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas",
                column: "CatogoryId",
                principalTable: "Catogorys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Submissions_SubmissionId",
                table: "Ideas",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Submissions_SubmissionId",
                table: "Ideas");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CatogoryId",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas",
                column: "CatogoryId",
                principalTable: "Catogorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Submissions_SubmissionId",
                table: "Ideas",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _80 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Ideas");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas");

            migrationBuilder.AlterColumn<int>(
                name: "CatogoryId",
                table: "Ideas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas",
                column: "CatogoryId",
                principalTable: "Catogorys",
                principalColumn: "Id");
        }
    }
}

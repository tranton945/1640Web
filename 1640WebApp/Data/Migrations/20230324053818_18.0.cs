using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _180 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentReaction",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "HahaCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "LoveCount",
                table: "Ideas");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Ideas");

            migrationBuilder.AddColumn<string>(
                name: "CurrentReaction",
                table: "Ideas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HahaCount",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoveCount",
                table: "Ideas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

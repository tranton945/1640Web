using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _150 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HahaCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "LoveCount",
                table: "Ideas");
        }
    }
}

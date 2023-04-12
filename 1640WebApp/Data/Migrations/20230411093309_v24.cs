using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class v24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Option1Count",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Option2Count",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Option3Count",
                table: "Votes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Option4Count",
                table: "Votes",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Option1Count",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Option2Count",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Option3Count",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "Option4Count",
                table: "Votes");
        }
    }
}

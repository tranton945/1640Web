using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _1640WebApp.Data.Migrations
{
    public partial class _20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatogoryId",
                table: "Ideas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Catogorys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catogorys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_CatogoryId",
                table: "Ideas",
                column: "CatogoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas",
                column: "CatogoryId",
                principalTable: "Catogorys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Catogorys_CatogoryId",
                table: "Ideas");

            migrationBuilder.DropTable(
                name: "Catogorys");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_CatogoryId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "CatogoryId",
                table: "Ideas");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Migrations
{
    public partial class addedSaltTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Salts",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Secret = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salts", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Salts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Salts");
        }
    }
}

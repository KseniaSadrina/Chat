using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Migrations
{
    public partial class senderTyoeToMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SenderType",
                table: "Messages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderType",
                table: "Messages");
        }
    }
}

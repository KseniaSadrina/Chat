using Microsoft.EntityFrameworkCore.Migrations;

namespace Chat.Migrations
{
    public partial class removedmigrationseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Scenarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Scenarios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Scenarios",
                keyColumn: "Id",
                keyValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Scenarios",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 1, "SQL injection is a code injection technique that might destroy your database.SQL injection is one of the most common web hacking techniques.SQL injection is the placement of malicious code in SQL statements, via web page input.", "SQLInjection" });

            migrationBuilder.InsertData(
                table: "Scenarios",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 2, "Slowloris is a type of denial of service attack tool invented by Robert 'RSnake' Hansen which allows a single machine to take down another machine's web server with minimal bandwidth and side effects on unrelated services and ports.", "Apache Shutdown" });

            migrationBuilder.InsertData(
                table: "Scenarios",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[] { 3, "Any malicious computer program which misleads users of its true intent. The term is derived from the Ancient Greek story of the deceptive wooden horse that led to the fall of the city of.", "Trojan" });
        }
    }
}

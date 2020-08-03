using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieLand.Data.Migrations
{
    public partial class AddCareersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Careers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Careers", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Careers",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "Director" });

            migrationBuilder.InsertData(
                table: "Careers",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2, "Actor" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Careers");
        }
    }
}

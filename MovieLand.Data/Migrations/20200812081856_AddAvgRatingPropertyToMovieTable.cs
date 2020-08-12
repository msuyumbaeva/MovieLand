using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieLand.Data.Migrations
{
    public partial class AddAvgRatingPropertyToMovieTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AvgRating",
                table: "Movies",
                nullable: false,
                defaultValueSql: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvgRating",
                table: "Movies");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieLand.Data.Migrations
{
    public partial class AddStarRatingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StarRatings",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Value = table.Column<byte>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StarRatings", x => new { x.MovieId, x.UserId });
                    table.ForeignKey(
                        name: "FK_StarRatings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StarRatings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StarRatings_MovieId",
                table: "StarRatings",
                column: "MovieId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StarRatings");
        }
    }
}

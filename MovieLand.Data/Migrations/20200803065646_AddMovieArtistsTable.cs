using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieLand.Data.Migrations
{
    public partial class AddMovieArtistsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieArtists",
                columns: table => new
                {
                    MovieId = table.Column<Guid>(nullable: false),
                    ArtistId = table.Column<Guid>(nullable: false),
                    CareerId = table.Column<int>(nullable: false),
                    Priority = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieArtists", x => new { x.MovieId, x.ArtistId, x.CareerId });
                    table.ForeignKey(
                        name: "FK_MovieArtists_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieArtists_Careers_CareerId",
                        column: x => x.CareerId,
                        principalTable: "Careers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieArtists_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieArtists_ArtistId",
                table: "MovieArtists",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieArtists_CareerId",
                table: "MovieArtists",
                column: "CareerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieArtists");
        }
    }
}

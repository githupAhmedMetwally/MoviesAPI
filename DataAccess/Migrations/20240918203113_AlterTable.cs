using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CinemaId",
                table: "movies",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_movies_CinemaId",
                table: "movies",
                column: "CinemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_movies_cinemas_CinemaId",
                table: "movies",
                column: "CinemaId",
                principalTable: "cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movies_cinemas_CinemaId",
                table: "movies");

            migrationBuilder.DropIndex(
                name: "IX_movies_CinemaId",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "movies");
        }
    }
}

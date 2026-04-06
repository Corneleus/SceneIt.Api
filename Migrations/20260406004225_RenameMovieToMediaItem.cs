using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneIt.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameMovieToMediaItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMovies_Movies_MovieId",
                table: "UserMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMovies_Users_UserId",
                table: "UserMovies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movies",
                table: "Movies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies");

            migrationBuilder.RenameTable(
                name: "Movies",
                newName: "MediaItems");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "MediaItems",
                newName: "MediaItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Movies_ImdbId",
                table: "MediaItems",
                newName: "IX_MediaItems_ImdbId");

            migrationBuilder.RenameTable(
                name: "UserMovies",
                newName: "UserMediaItems");

            migrationBuilder.RenameColumn(
                name: "UserMovieId",
                table: "UserMediaItems",
                newName: "UserMediaItemId");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "UserMediaItems",
                newName: "MediaItemId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMovies_UserId",
                table: "UserMediaItems",
                newName: "IX_UserMediaItems_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMovies_MovieId",
                table: "UserMediaItems",
                newName: "IX_UserMediaItems_MediaItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MediaItems",
                table: "MediaItems",
                column: "MediaItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMediaItems",
                table: "UserMediaItems",
                column: "UserMediaItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMediaItems_MediaItems_MediaItemId",
                table: "UserMediaItems",
                column: "MediaItemId",
                principalTable: "MediaItems",
                principalColumn: "MediaItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMediaItems_Users_UserId",
                table: "UserMediaItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMediaItems_MediaItems_MediaItemId",
                table: "UserMediaItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserMediaItems_Users_UserId",
                table: "UserMediaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MediaItems",
                table: "MediaItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMediaItems",
                table: "UserMediaItems");

            migrationBuilder.RenameTable(
                name: "MediaItems",
                newName: "Movies");

            migrationBuilder.RenameColumn(
                name: "MediaItemId",
                table: "Movies",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_MediaItems_ImdbId",
                table: "Movies",
                newName: "IX_Movies_ImdbId");

            migrationBuilder.RenameTable(
                name: "UserMediaItems",
                newName: "UserMovies");

            migrationBuilder.RenameColumn(
                name: "UserMediaItemId",
                table: "UserMovies",
                newName: "UserMovieId");

            migrationBuilder.RenameColumn(
                name: "MediaItemId",
                table: "UserMovies",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMediaItems_UserId",
                table: "UserMovies",
                newName: "IX_UserMovies_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserMediaItems_MediaItemId",
                table: "UserMovies",
                newName: "IX_UserMovies_MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movies",
                table: "Movies",
                column: "MovieId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMovies",
                table: "UserMovies",
                column: "UserMovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMovies_Movies_MovieId",
                table: "UserMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserMovies_Users_UserId",
                table: "UserMovies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

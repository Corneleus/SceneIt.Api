using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SceneIt.Api.Migrations
{
    public partial class ImportQueueDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Actors",
                table: "ImportQueueItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Awards",
                table: "ImportQueueItems",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BoxOffice",
                table: "ImportQueueItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ImportQueueItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dvd",
                table: "ImportQueueItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "ImportQueueItems",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbRating",
                table: "ImportQueueItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImdbVotes",
                table: "ImportQueueItems",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "ImportQueueItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Metascore",
                table: "ImportQueueItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plot",
                table: "ImportQueueItems",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Production",
                table: "ImportQueueItems",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Poster",
                table: "ImportQueueItems",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rated",
                table: "ImportQueueItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Runtime",
                table: "ImportQueueItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ImportQueueItems",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Writer",
                table: "ImportQueueItems",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "ImportQueueItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Actors", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Awards", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "BoxOffice", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Country", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Dvd", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Genre", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "ImdbRating", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "ImdbVotes", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Language", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Metascore", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Plot", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Production", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Poster", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Rated", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Runtime", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Type", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Writer", table: "ImportQueueItems");
            migrationBuilder.DropColumn(name: "Year", table: "ImportQueueItems");
        }
    }
}

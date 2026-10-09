using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookTracker.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDescriptionAuthorsAndGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Authors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Genres",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Authors",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

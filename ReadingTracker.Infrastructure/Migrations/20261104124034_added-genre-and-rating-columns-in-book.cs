using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadingTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedgenreandratingcolumnsinbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "Books",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Books");
        }
    }
}

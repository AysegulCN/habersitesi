using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace habersite.Migrations
{
    /// <inheritdoc />
    public partial class AddSlugToNews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "News");
        }
    }
}

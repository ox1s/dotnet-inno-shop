using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnoShop.ProductManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExplicitConditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "products",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "products",
                newName: "description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "title",
                table: "products",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "products",
                newName: "Description");
        }
    }
}

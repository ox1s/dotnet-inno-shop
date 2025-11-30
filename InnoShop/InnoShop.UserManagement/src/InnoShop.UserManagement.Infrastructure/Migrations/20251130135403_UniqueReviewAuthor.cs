using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnoShop.UserManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueReviewAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reviews_author_id",
                table: "reviews");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_author_id_target_user_id",
                table: "reviews",
                columns: new[] { "author_id", "target_user_id" },
                unique: true,
                filter: "\"is_deleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_reviews_author_id_target_user_id",
                table: "reviews");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_author_id",
                table: "reviews",
                column: "author_id");
        }
    }
}

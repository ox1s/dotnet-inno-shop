using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InnoShop.ProductManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SellerInfo_ReviewCount",
                table: "products",
                newName: "seller_review_count");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "seller_review_count",
                table: "products",
                newName: "SellerInfo_ReviewCount");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierEmailToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplierEmail",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierEmail",
                table: "Products");
        }
    }
}

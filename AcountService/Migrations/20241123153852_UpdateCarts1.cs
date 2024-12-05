using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanVatLieuXayDung.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarts1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "code_discount",
                table: "Carts",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "promotion_discount",
                table: "Carts",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code_discount",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "promotion_discount",
                table: "Carts");
        }
    }
}

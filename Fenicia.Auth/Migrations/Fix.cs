#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount",
                table: "order_details",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "modules",
                newName: "price");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "order_details",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "modules",
                newName: "amount");
        }
    }
}
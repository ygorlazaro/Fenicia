#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class Fix : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "amount",
            "order_details",
            "price");

        migrationBuilder.RenameColumn(
            "amount",
            "modules",
            "price");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "price",
            "order_details",
            "amount");

        migrationBuilder.RenameColumn(
            "price",
            "modules",
            "amount");
    }
}
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class AddUploadsInAuthSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            name: "uploads",
            schema: "public",
            newName: "uploads",
            newSchema: "auth");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "public");

        migrationBuilder.RenameTable(
            name: "uploads",
            schema: "auth",
            newName: "uploads",
            newSchema: "public");
    }
}

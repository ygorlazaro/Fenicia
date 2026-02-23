#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class Person : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            "zip_code",
            "addresses",
            "character varying(8)",
            maxLength: 8,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(9)",
            oldMaxLength: 9);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            "zip_code",
            "addresses",
            "character varying(9)",
            maxLength: 9,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(8)",
            oldMaxLength: 8);
    }
}
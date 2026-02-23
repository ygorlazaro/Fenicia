#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class Submodule : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "submodules",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                route = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                description = table.Column<string>("character varying(100)", maxLength: 100, nullable: true),
                module_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_submodules", x => x.id);
                table.ForeignKey(
                    "fk_submodules_modules_module_id",
                    x => x.module_id,
                    "modules",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_submodules_module_id",
            "submodules",
            "module_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "submodules");
    }
}
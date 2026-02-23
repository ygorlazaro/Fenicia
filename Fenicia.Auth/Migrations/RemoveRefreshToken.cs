#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class RemoveRefreshToken : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "refresh_tokens");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "refresh_tokens",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                expiration_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                token = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    "fk_refresh_tokens_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_refresh_tokens_user_id",
            "refresh_tokens",
            "user_id");
    }
}
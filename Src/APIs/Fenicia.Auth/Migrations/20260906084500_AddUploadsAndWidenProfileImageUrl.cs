using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

public partial class AddUploadsAndWidenProfileImageUrl : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "image_url",
            schema: "social_network",
            table: "profiles",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(48)",
            oldMaxLength: 48,
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "uploads",
            schema: "public",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                original_file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                content_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                size_bytes = table.Column<long>(type: "bigint", nullable: false),
                url = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_uploads", x => x.id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "uploads",
            schema: "public");

        migrationBuilder.AlterColumn<string>(
            name: "image_url",
            schema: "social_network",
            table: "profiles",
            type: "character varying(48)",
            maxLength: 48,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "character varying(200)",
            oldMaxLength: 200,
            oldNullable: true);
    }
}

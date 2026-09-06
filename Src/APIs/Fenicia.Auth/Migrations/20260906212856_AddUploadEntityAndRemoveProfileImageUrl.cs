using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class AddUploadEntityAndRemoveProfileImageUrl : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "image_url",
            schema: "social_network",
            table: "profiles");

        migrationBuilder.EnsureSchema(
            name: "auth");

        migrationBuilder.AddColumn<Guid>(
            name: "upload_id",
            schema: "social_network",
            table: "profiles",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "uploads",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                original_file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                stored_file_name = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: false),
                content_type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                size_bytes = table.Column<long>(type: "bigint", nullable: false),
                url = table.Column<string>(type: "character varying(260)", maxLength: 260, nullable: true),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_uploads", x => x.id);
            });

        migrationBuilder.CreateIndex(
            name: "ix_profiles_upload_id",
            schema: "social_network",
            table: "profiles",
            column: "upload_id");

        migrationBuilder.AddForeignKey(
            name: "fk_profiles_uploads_upload_id",
            schema: "social_network",
            table: "profiles",
            column: "upload_id",
            principalSchema: "auth",
            principalTable: "uploads",
            principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_profiles_uploads_upload_id",
            schema: "social_network",
            table: "profiles");

        migrationBuilder.DropTable(
            name: "uploads",
            schema: "auth");

        migrationBuilder.DropIndex(
            name: "ix_profiles_upload_id",
            schema: "social_network",
            table: "profiles");

        migrationBuilder.DropColumn(
            name: "upload_id",
            schema: "social_network",
            table: "profiles");

        migrationBuilder.AddColumn<string>(
            name: "image_url",
            schema: "social_network",
            table: "profiles",
            type: "character varying(200)",
            maxLength: 200,
            nullable: true);
    }
}

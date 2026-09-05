using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class FixSocialNetworkCommentsForeignKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_comments_users_user_id",
            schema: "social_network",
            table: "comments");

        migrationBuilder.AddForeignKey(
            name: "fk_comments_profiles_profile_id",
            schema: "social_network",
            table: "comments",
            column: "profile_id",
            principalSchema: "social_network",
            principalTable: "profiles",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "fk_comments_profiles_profile_id",
            schema: "social_network",
            table: "comments");

        migrationBuilder.AddForeignKey(
            name: "fk_comments_users_user_id",
            schema: "social_network",
            table: "comments",
            column: "user_id",
            principalSchema: "auth",
            principalTable: "users",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}

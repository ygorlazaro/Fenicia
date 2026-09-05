using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class ConvertSocialNetworkUserIdToProfileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_blocks_users_blocked_user_id",
                schema: "social_network",
                table: "blocks");

            migrationBuilder.DropForeignKey(
                name: "fk_blocks_users_user_id",
                schema: "social_network",
                table: "blocks");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_feeds_users_user_id",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropForeignKey(
                name: "fk_friendships_users_target_user_id",
                schema: "social_network",
                table: "friendships");

            migrationBuilder.DropForeignKey(
                name: "fk_friendships_users_user_id",
                schema: "social_network",
                table: "friendships");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_users_user_id",
                schema: "social_network",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_shares_users_user_id",
                schema: "social_network",
                table: "shares");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "shares",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_shares_user_id",
                schema: "social_network",
                table: "shares",
                newName: "ix_shares_profile_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "likes",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_likes_user_id",
                schema: "social_network",
                table: "likes",
                newName: "ix_likes_profile_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "friendships",
                newName: "target_profile_id");

            migrationBuilder.RenameColumn(
                name: "target_user_id",
                schema: "social_network",
                table: "friendships",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_friendships_user_id",
                schema: "social_network",
                table: "friendships",
                newName: "ix_friendships_target_profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_friendships_target_user_id",
                schema: "social_network",
                table: "friendships",
                newName: "ix_friendships_profile_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "feeds",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_feeds_user_id",
                schema: "social_network",
                table: "feeds",
                newName: "ix_feeds_profile_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "comments",
                newName: "profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_user_id1",
                schema: "social_network",
                table: "comments",
                newName: "ix_comments_profile_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "social_network",
                table: "blocks",
                newName: "profile_id");

            migrationBuilder.RenameColumn(
                name: "blocked_user_id",
                schema: "social_network",
                table: "blocks",
                newName: "blocked_profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_blocks_user_id",
                schema: "social_network",
                table: "blocks",
                newName: "ix_blocks_profile_id");

            migrationBuilder.RenameIndex(
                name: "ix_blocks_blocked_user_id",
                schema: "social_network",
                table: "blocks",
                newName: "ix_blocks_blocked_profile_id");

            migrationBuilder.AddForeignKey(
                name: "fk_blocks_profiles_blocked_profile_id",
                schema: "social_network",
                table: "blocks",
                column: "blocked_profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_blocks_profiles_profile_id",
                schema: "social_network",
                table: "blocks",
                column: "profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_profiles_profile_id",
                schema: "social_network",
                table: "comments",
                column: "profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_feeds_profiles_profile_id",
                schema: "social_network",
                table: "feeds",
                column: "profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_friendships_profiles_profile_id",
                schema: "social_network",
                table: "friendships",
                column: "profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_friendships_profiles_target_profile_id",
                schema: "social_network",
                table: "friendships",
                column: "target_profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_likes_profiles_profile_id",
                schema: "social_network",
                table: "likes",
                column: "profile_id",
                principalSchema: "social_network",
                principalTable: "profiles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shares_profiles_profile_id",
                schema: "social_network",
                table: "shares",
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
                name: "fk_blocks_profiles_blocked_profile_id",
                schema: "social_network",
                table: "blocks");

            migrationBuilder.DropForeignKey(
                name: "fk_blocks_profiles_profile_id",
                schema: "social_network",
                table: "blocks");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_profiles_profile_id",
                schema: "social_network",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_feeds_profiles_profile_id",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropForeignKey(
                name: "fk_friendships_profiles_profile_id",
                schema: "social_network",
                table: "friendships");

            migrationBuilder.DropForeignKey(
                name: "fk_friendships_profiles_target_profile_id",
                schema: "social_network",
                table: "friendships");

            migrationBuilder.DropForeignKey(
                name: "fk_likes_profiles_profile_id",
                schema: "social_network",
                table: "likes");

            migrationBuilder.DropForeignKey(
                name: "fk_shares_profiles_profile_id",
                schema: "social_network",
                table: "shares");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "shares",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_shares_profile_id",
                schema: "social_network",
                table: "shares",
                newName: "ix_shares_user_id");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "likes",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_likes_profile_id",
                schema: "social_network",
                table: "likes",
                newName: "ix_likes_user_id");

            migrationBuilder.RenameColumn(
                name: "target_profile_id",
                schema: "social_network",
                table: "friendships",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "friendships",
                newName: "target_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_friendships_target_profile_id",
                schema: "social_network",
                table: "friendships",
                newName: "ix_friendships_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_friendships_profile_id",
                schema: "social_network",
                table: "friendships",
                newName: "ix_friendships_target_user_id");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "feeds",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_feeds_profile_id",
                schema: "social_network",
                table: "feeds",
                newName: "ix_feeds_user_id");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "comments",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_comments_profile_id",
                schema: "social_network",
                table: "comments",
                newName: "ix_comments_user_id1");

            migrationBuilder.RenameColumn(
                name: "profile_id",
                schema: "social_network",
                table: "blocks",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "blocked_profile_id",
                schema: "social_network",
                table: "blocks",
                newName: "blocked_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_blocks_profile_id",
                schema: "social_network",
                table: "blocks",
                newName: "ix_blocks_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_blocks_blocked_profile_id",
                schema: "social_network",
                table: "blocks",
                newName: "ix_blocks_blocked_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_blocks_users_blocked_user_id",
                schema: "social_network",
                table: "blocks",
                column: "blocked_user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_blocks_users_user_id",
                schema: "social_network",
                table: "blocks",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_id",
                schema: "social_network",
                table: "comments",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_feeds_users_user_id",
                schema: "social_network",
                table: "feeds",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_friendships_users_target_user_id",
                schema: "social_network",
                table: "friendships",
                column: "target_user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_friendships_users_user_id",
                schema: "social_network",
                table: "friendships",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_likes_users_user_id",
                schema: "social_network",
                table: "likes",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shares_users_user_id",
                schema: "social_network",
                table: "shares",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }

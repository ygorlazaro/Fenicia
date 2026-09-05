using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

    /// <inheritdoc />
    public partial class AddFeedOriginalFeedId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "original_feed_id",
                schema: "social_network",
                table: "feeds",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_feeds_original_feed_id",
                schema: "social_network",
                table: "feeds",
                column: "original_feed_id");

            migrationBuilder.AddForeignKey(
                name: "fk_feeds_feeds_original_feed_id",
                schema: "social_network",
                table: "feeds",
                column: "original_feed_id",
                principalSchema: "social_network",
                principalTable: "feeds",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_feeds_feeds_original_feed_id",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropIndex(
                name: "ix_feeds_original_feed_id",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "original_feed_id",
                schema: "social_network",
                table: "feeds");
        }
    }

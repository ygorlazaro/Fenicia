using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

    /// <inheritdoc />
    public partial class AddFeedTotalLikesCommentsShares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "total_comments",
                schema: "social_network",
                table: "feeds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_likes",
                schema: "social_network",
                table: "feeds",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_shares",
                schema: "social_network",
                table: "feeds",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_comments",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "total_likes",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "total_shares",
                schema: "social_network",
                table: "feeds");
        }
    }

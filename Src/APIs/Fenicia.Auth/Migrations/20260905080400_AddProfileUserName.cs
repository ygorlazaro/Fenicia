using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

    /// <inheritdoc />
    public partial class AddProfileUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_name",
                schema: "social_network",
                table: "profiles",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_name",
                schema: "social_network",
                table: "profiles");
        }
    }

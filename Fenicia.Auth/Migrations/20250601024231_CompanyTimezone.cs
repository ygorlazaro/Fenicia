using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class CompanyTimezone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "companies",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "companies",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "logo",
                table: "companies",
                type: "text",
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<string>(
                name: "time_zone",
                table: "companies",
                type: "text",
                nullable: false,
                defaultValue: ""
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "is_active", table: "companies");

            migrationBuilder.DropColumn(name: "language", table: "companies");

            migrationBuilder.DropColumn(name: "logo", table: "companies");

            migrationBuilder.DropColumn(name: "time_zone", table: "companies");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleMulti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_roles_roles_id",
                table: "users_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_users_users_id",
                table: "users_roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users_roles",
                table: "users_roles");

            migrationBuilder.RenameColumn(
                name: "users_id",
                table: "users_roles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "roles_id",
                table: "users_roles",
                newName: "role_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_roles_users_id",
                table: "users_roles",
                newName: "ix_users_roles_user_id");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                table: "users_roles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "created",
                table: "users_roles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted",
                table: "users_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                table: "users_roles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_users_roles",
                table: "users_roles",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_users_roles_role_id",
                table: "users_roles",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_roles_role_id",
                table: "users_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_users_user_id",
                table: "users_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_roles_role_id",
                table: "users_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_users_roles_users_user_id",
                table: "users_roles");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users_roles",
                table: "users_roles");

            migrationBuilder.DropIndex(
                name: "ix_users_roles_role_id",
                table: "users_roles");

            migrationBuilder.DropColumn(
                name: "id",
                table: "users_roles");

            migrationBuilder.DropColumn(
                name: "created",
                table: "users_roles");

            migrationBuilder.DropColumn(
                name: "deleted",
                table: "users_roles");

            migrationBuilder.DropColumn(
                name: "updated",
                table: "users_roles");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "users_roles",
                newName: "users_id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "users_roles",
                newName: "roles_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_roles_user_id",
                table: "users_roles",
                newName: "ix_users_roles_users_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users_roles",
                table: "users_roles",
                columns: new[] { "roles_id", "users_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_roles_roles_id",
                table: "users_roles",
                column: "roles_id",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_users_roles_users_users_id",
                table: "users_roles",
                column: "users_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

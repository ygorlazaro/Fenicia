using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

    /// <inheritdoc />
    public partial class FixProjectStatusForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_statuses_projects_project_model_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.DropIndex(
                name: "ix_statuses_project_model_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.DropColumn(
                name: "project_model_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.CreateIndex(
                name: "ix_statuses_project_id",
                schema: "project",
                table: "statuses",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_statuses_projects_project_id",
                schema: "project",
                table: "statuses",
                column: "project_id",
                principalSchema: "project",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_statuses_projects_project_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.DropIndex(
                name: "ix_statuses_project_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.AddColumn<Guid>(
                name: "project_model_id",
                schema: "project",
                table: "statuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_statuses_project_model_id",
                schema: "project",
                table: "statuses",
                column: "project_model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_statuses_projects_project_model_id",
                schema: "project",
                table: "statuses",
                column: "project_model_id",
                principalSchema: "project",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }

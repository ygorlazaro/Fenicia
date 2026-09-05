using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class AddSprint : Migration
{
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "sprint_id",
                schema: "project",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sprints",
                schema: "project",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sprints", x => x.id);
                    table.ForeignKey(
                        name: "fk_sprints_projects_project_id",
                        column: x => x.project_id,
                        principalSchema: "project",
                        principalTable: "projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sprints_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tasks_sprint_id",
                schema: "project",
                table: "tasks",
                column: "sprint_id");

            migrationBuilder.CreateIndex(
                name: "ix_sprints_created_by",
                schema: "project",
                table: "sprints",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_sprints_project_id",
                schema: "project",
                table: "sprints",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_sprints_sprint_id",
                schema: "project",
                table: "tasks",
                column: "sprint_id",
                principalSchema: "project",
                principalTable: "sprints",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tasks_sprints_sprint_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "sprints",
                schema: "project");

            migrationBuilder.DropIndex(
                name: "ix_tasks_sprint_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "sprint_id",
                schema: "project",
                table: "tasks");
        }
    }

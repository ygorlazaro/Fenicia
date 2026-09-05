using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

    /// <inheritdoc />
    public partial class FixProjectForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_tasks_task_model_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_users_user_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_tasks_task_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_project_subtasks_tasks_task_model_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropForeignKey(
                name: "fk_task_assignees_tasks_task_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_projects_project_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_statuses_status_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_users_user_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_project_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_status_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_user_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_task_assignees_task_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropIndex(
                name: "ix_project_subtasks_task_model_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropIndex(
                name: "ix_comments_task_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_task_model_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_user_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "project_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "status_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "task_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropColumn(
                name: "task_model_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropColumn(
                name: "task_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "task_model_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_created_by",
                schema: "project",
                table: "tasks",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_project_id",
                schema: "project",
                table: "tasks",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_status_id",
                schema: "project",
                table: "tasks",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_task_assignees_task_id",
                schema: "project",
                table: "task_assignees",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_subtasks_task_id",
                schema: "project",
                table: "project_subtasks",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_task_id",
                schema: "project",
                table: "comments",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_task_id",
                schema: "project",
                table: "attachments",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_uploaded_by",
                schema: "project",
                table: "attachments",
                column: "uploaded_by");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_tasks_task_id",
                schema: "project",
                table: "attachments",
                column: "task_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_users_uploaded_by",
                schema: "project",
                table: "attachments",
                column: "uploaded_by",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_tasks_task_id",
                schema: "project",
                table: "comments",
                column: "task_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_project_subtasks_tasks_task_id",
                schema: "project",
                table: "project_subtasks",
                column: "task_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_assignees_tasks_task_id",
                schema: "project",
                table: "task_assignees",
                column: "task_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_projects_project_id",
                schema: "project",
                table: "tasks",
                column: "project_id",
                principalSchema: "project",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_statuses_status_id",
                schema: "project",
                table: "tasks",
                column: "status_id",
                principalSchema: "project",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_users_created_by",
                schema: "project",
                table: "tasks",
                column: "created_by",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_tasks_task_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_attachments_users_uploaded_by",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_tasks_task_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_project_subtasks_tasks_task_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropForeignKey(
                name: "fk_task_assignees_tasks_task_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_projects_project_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_statuses_status_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_users_created_by",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_created_by",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_project_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_tasks_status_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "ix_task_assignees_task_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropIndex(
                name: "ix_project_subtasks_task_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropIndex(
                name: "ix_comments_task_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_task_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropIndex(
                name: "ix_attachments_uploaded_by",
                schema: "project",
                table: "attachments");

            migrationBuilder.AddColumn<Guid>(
                name: "project_model_id",
                schema: "project",
                table: "tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "status_model_id",
                schema: "project",
                table: "tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "project",
                table: "tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "task_model_id",
                schema: "project",
                table: "task_assignees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "task_model_id",
                schema: "project",
                table: "project_subtasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "task_model_id",
                schema: "project",
                table: "comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "task_model_id",
                schema: "project",
                table: "attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "project",
                table: "attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_tasks_project_model_id",
                schema: "project",
                table: "tasks",
                column: "project_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_status_model_id",
                schema: "project",
                table: "tasks",
                column: "status_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_tasks_user_id",
                schema: "project",
                table: "tasks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_task_assignees_task_model_id",
                schema: "project",
                table: "task_assignees",
                column: "task_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_project_subtasks_task_model_id",
                schema: "project",
                table: "project_subtasks",
                column: "task_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_task_model_id",
                schema: "project",
                table: "comments",
                column: "task_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_task_model_id",
                schema: "project",
                table: "attachments",
                column: "task_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_user_id",
                schema: "project",
                table: "attachments",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_tasks_task_model_id",
                schema: "project",
                table: "attachments",
                column: "task_model_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_users_user_id",
                schema: "project",
                table: "attachments",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_tasks_task_model_id",
                schema: "project",
                table: "comments",
                column: "task_model_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_project_subtasks_tasks_task_model_id",
                schema: "project",
                table: "project_subtasks",
                column: "task_model_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_task_assignees_tasks_task_model_id",
                schema: "project",
                table: "task_assignees",
                column: "task_model_id",
                principalSchema: "project",
                principalTable: "tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_projects_project_model_id",
                schema: "project",
                table: "tasks",
                column: "project_model_id",
                principalSchema: "project",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_statuses_status_model_id",
                schema: "project",
                table: "tasks",
                column: "status_model_id",
                principalSchema: "project",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_users_user_id",
                schema: "project",
                table: "tasks",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }

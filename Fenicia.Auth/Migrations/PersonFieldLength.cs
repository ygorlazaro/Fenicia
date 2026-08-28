#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class PersonFieldLength : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("fk_attachments_tasks_task_id", schema: "project", table: "attachments");

        migrationBuilder.DropForeignKey("fk_attachments_users_user_id", schema: "project", table: "attachments");

        migrationBuilder.DropForeignKey("fk_comments_tasks_task_id", schema: "project", table: "comments");

        migrationBuilder.DropForeignKey("fk_comments_users_user_id", schema: "project", table: "comments");

        migrationBuilder.DropForeignKey("fk_feeds_users_user_id", schema: "social_network", table: "feeds");

        migrationBuilder.DropForeignKey("fk_project_subtasks_tasks_task_id", schema: "project", table: "project_subtasks");

        migrationBuilder.DropForeignKey("fk_statuses_projects_project_id", schema: "project", table: "statuses");

        migrationBuilder.DropForeignKey("fk_suppliers_people_person_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropForeignKey("fk_task_assignees_tasks_task_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropForeignKey("fk_tasks_projects_project_id", schema: "project", table: "tasks");

        migrationBuilder.DropForeignKey("fk_tasks_statuses_status_id", schema: "project", table: "tasks");

        migrationBuilder.DropForeignKey("fk_tasks_users_user_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_tasks_project_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_tasks_status_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_task_assignees_task_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropIndex("ix_suppliers_person_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropIndex("ix_statuses_project_id", schema: "project", table: "statuses");

        migrationBuilder.DropIndex("ix_project_subtasks_task_id", schema: "project", table: "project_subtasks");

        migrationBuilder.DropIndex("ix_feeds_user_id", schema: "social_network", table: "feeds");

        migrationBuilder.DropIndex("ix_comments_task_id", schema: "project", table: "comments");

        migrationBuilder.DropIndex("ix_comments_user_id", schema: "project", table: "comments");

        migrationBuilder.DropIndex("ix_attachments_task_id", schema: "project", table: "attachments");

        migrationBuilder.RenameColumn("user_id", schema: "project", table: "tasks", newName: "user_model_id");

        migrationBuilder.RenameIndex("ix_tasks_user_id", schema: "project", table: "tasks", newName: "ix_tasks_user_model_id");

        migrationBuilder.RenameColumn("user_id", schema: "project", table: "attachments", newName: "user_model_id");

        migrationBuilder.RenameIndex("ix_attachments_user_id", schema: "project", table: "attachments", newName: "ix_attachments_user_model_id");

        migrationBuilder.AddColumn<Guid>("project_model_id", schema: "project", table: "tasks", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("status_model_id", schema: "project", table: "tasks", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("task_model_id", schema: "project", table: "task_assignees", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("user_model_id", schema: "project", table: "task_assignees", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("person_model_id", schema: "basic", table: "suppliers", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("project_model_id", schema: "project", table: "statuses", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<DateTime>("due_date", schema: "project", table: "project_subtasks", type: "timestamp with time zone", nullable: true);

        migrationBuilder.AddColumn<Guid>("task_model_id", schema: "project", table: "project_subtasks", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AlterColumn<string>("number", schema: "basic", table: "people", type: "character varying(20)", maxLength: 20, nullable: true, oldClrType: typeof(string), oldType: "character varying(10)", oldMaxLength: 10, oldNullable: true);

        migrationBuilder.AlterColumn<string>("complement", schema: "basic", table: "people", type: "character varying(20)", maxLength: 20, nullable: true, oldClrType: typeof(string), oldType: "character varying(10)", oldMaxLength: 10, oldNullable: true);

        migrationBuilder.AddColumn<Guid>("user_model_id", schema: "social_network", table: "feeds", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("author_id", schema: "project", table: "comments", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("task_model_id", schema: "project", table: "comments", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>("user_model_id", schema: "project", table: "comments", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<string>("content_type", schema: "project", table: "attachments", type: "text", nullable: false, defaultValue: string.Empty);

        migrationBuilder.AddColumn<long>("size", schema: "project", table: "attachments", type: "bigint", nullable: false, defaultValue: 0L);

        migrationBuilder.AddColumn<Guid>("task_model_id", schema: "project", table: "attachments", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex("ix_tasks_project_model_id", schema: "project", table: "tasks", column: "project_model_id");

        migrationBuilder.CreateIndex("ix_tasks_status_model_id", schema: "project", table: "tasks", column: "status_model_id");

        migrationBuilder.CreateIndex("ix_task_assignees_task_model_id", schema: "project", table: "task_assignees", column: "task_model_id");

        migrationBuilder.CreateIndex("ix_task_assignees_user_model_id", schema: "project", table: "task_assignees", column: "user_model_id");

        migrationBuilder.CreateIndex("ix_suppliers_person_model_id", schema: "basic", table: "suppliers", column: "person_model_id");

        migrationBuilder.CreateIndex("ix_statuses_project_model_id", schema: "project", table: "statuses", column: "project_model_id");

        migrationBuilder.CreateIndex("ix_project_subtasks_task_model_id", schema: "project", table: "project_subtasks", column: "task_model_id");

        migrationBuilder.CreateIndex("ix_feeds_user_model_id", schema: "social_network", table: "feeds", column: "user_model_id");

        migrationBuilder.CreateIndex("ix_comments_task_model_id", schema: "project", table: "comments", column: "task_model_id");

        migrationBuilder.CreateIndex("ix_comments_user_model_id", schema: "project", table: "comments", column: "user_model_id");

        migrationBuilder.CreateIndex("ix_attachments_task_model_id", schema: "project", table: "attachments", column: "task_model_id");

        migrationBuilder.AddForeignKey("fk_attachments_tasks_task_model_id", schema: "project", table: "attachments", column: "task_model_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_attachments_users_user_model_id", schema: "project", table: "attachments", column: "user_model_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_comments_tasks_task_model_id", schema: "project", table: "comments", column: "task_model_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_comments_users_user_model_id", schema: "project", table: "comments", column: "user_model_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_feeds_users_user_model_id", schema: "social_network", table: "feeds", column: "user_model_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_project_subtasks_tasks_task_model_id", schema: "project", table: "project_subtasks", column: "task_model_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_statuses_projects_project_model_id", schema: "project", table: "statuses", column: "project_model_id", principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_suppliers_people_person_model_id", schema: "basic", table: "suppliers", column: "person_model_id", principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_task_assignees_tasks_task_model_id", schema: "project", table: "task_assignees", column: "task_model_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_task_assignees_users_user_model_id", schema: "project", table: "task_assignees", column: "user_model_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_projects_project_model_id", schema: "project", table: "tasks", column: "project_model_id", principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_statuses_status_model_id", schema: "project", table: "tasks", column: "status_model_id", principalSchema: "project", principalTable: "statuses", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_users_user_model_id", schema: "project", table: "tasks", column: "user_model_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("fk_attachments_tasks_task_model_id", schema: "project", table: "attachments");

        migrationBuilder.DropForeignKey("fk_attachments_users_user_model_id", schema: "project", table: "attachments");

        migrationBuilder.DropForeignKey("fk_comments_tasks_task_model_id", schema: "project", table: "comments");

        migrationBuilder.DropForeignKey("fk_comments_users_user_model_id", schema: "project", table: "comments");

        migrationBuilder.DropForeignKey("fk_feeds_users_user_model_id", schema: "social_network", table: "feeds");

        migrationBuilder.DropForeignKey("fk_project_subtasks_tasks_task_model_id", schema: "project", table: "project_subtasks");

        migrationBuilder.DropForeignKey("fk_statuses_projects_project_model_id", schema: "project", table: "statuses");

        migrationBuilder.DropForeignKey("fk_suppliers_people_person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropForeignKey("fk_task_assignees_tasks_task_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropForeignKey("fk_task_assignees_users_user_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropForeignKey("fk_tasks_projects_project_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropForeignKey("fk_tasks_statuses_status_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropForeignKey("fk_tasks_users_user_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_tasks_project_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_tasks_status_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropIndex("ix_task_assignees_task_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropIndex("ix_task_assignees_user_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropIndex("ix_suppliers_person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropIndex("ix_statuses_project_model_id", schema: "project", table: "statuses");

        migrationBuilder.DropIndex("ix_project_subtasks_task_model_id", schema: "project", table: "project_subtasks");

        migrationBuilder.DropIndex("ix_feeds_user_model_id", schema: "social_network", table: "feeds");

        migrationBuilder.DropIndex("ix_comments_task_model_id", schema: "project", table: "comments");

        migrationBuilder.DropIndex("ix_comments_user_model_id", schema: "project", table: "comments");

        migrationBuilder.DropIndex("ix_attachments_task_model_id", schema: "project", table: "attachments");

        migrationBuilder.DropColumn("project_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropColumn("status_model_id", schema: "project", table: "tasks");

        migrationBuilder.DropColumn("task_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropColumn("user_model_id", schema: "project", table: "task_assignees");

        migrationBuilder.DropColumn("person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropColumn("project_model_id", schema: "project", table: "statuses");

        migrationBuilder.DropColumn("due_date", schema: "project", table: "project_subtasks");

        migrationBuilder.DropColumn("task_model_id", schema: "project", table: "project_subtasks");

        migrationBuilder.DropColumn("user_model_id", schema: "social_network", table: "feeds");

        migrationBuilder.DropColumn("author_id", schema: "project", table: "comments");

        migrationBuilder.DropColumn("task_model_id", schema: "project", table: "comments");

        migrationBuilder.DropColumn("user_model_id", schema: "project", table: "comments");

        migrationBuilder.DropColumn("content_type", schema: "project", table: "attachments");

        migrationBuilder.DropColumn("size", schema: "project", table: "attachments");

        migrationBuilder.DropColumn("task_model_id", schema: "project", table: "attachments");

        migrationBuilder.RenameColumn("user_model_id", schema: "project", table: "tasks", newName: "user_id");

        migrationBuilder.RenameIndex("ix_tasks_user_model_id", schema: "project", table: "tasks", newName: "ix_tasks_user_id");

        migrationBuilder.RenameColumn("user_model_id", schema: "project", table: "attachments", newName: "user_id");

        migrationBuilder.RenameIndex("ix_attachments_user_model_id", schema: "project", table: "attachments", newName: "ix_attachments_user_id");

        migrationBuilder.AlterColumn<string>("number", schema: "basic", table: "people", type: "character varying(10)", maxLength: 10, nullable: true, oldClrType: typeof(string), oldType: "character varying(20)", oldMaxLength: 20, oldNullable: true);

        migrationBuilder.AlterColumn<string>("complement", schema: "basic", table: "people", type: "character varying(10)", maxLength: 10, nullable: true, oldClrType: typeof(string), oldType: "character varying(20)", oldMaxLength: 20, oldNullable: true);

        migrationBuilder.CreateIndex("ix_tasks_project_id", schema: "project", table: "tasks", column: "project_id");

        migrationBuilder.CreateIndex("ix_tasks_status_id", schema: "project", table: "tasks", column: "status_id");

        migrationBuilder.CreateIndex("ix_task_assignees_task_id", schema: "project", table: "task_assignees", column: "task_id");

        migrationBuilder.CreateIndex("ix_suppliers_person_id", schema: "basic", table: "suppliers", column: "person_id");

        migrationBuilder.CreateIndex("ix_statuses_project_id", schema: "project", table: "statuses", column: "project_id");

        migrationBuilder.CreateIndex("ix_project_subtasks_task_id", schema: "project", table: "project_subtasks", column: "task_id");

        migrationBuilder.CreateIndex("ix_feeds_user_id", schema: "social_network", table: "feeds", column: "user_id");

        migrationBuilder.CreateIndex("ix_comments_task_id", schema: "project", table: "comments", column: "task_id");

        migrationBuilder.CreateIndex("ix_comments_user_id", schema: "project", table: "comments", column: "user_id");

        migrationBuilder.CreateIndex("ix_attachments_task_id", schema: "project", table: "attachments", column: "task_id");

        migrationBuilder.AddForeignKey("fk_attachments_tasks_task_id", schema: "project", table: "attachments", column: "task_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_attachments_users_user_id", schema: "project", table: "attachments", column: "user_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_comments_tasks_task_id", schema: "project", table: "comments", column: "task_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_comments_users_user_id", schema: "project", table: "comments", column: "user_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_feeds_users_user_id", schema: "social_network", table: "feeds", column: "user_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_project_subtasks_tasks_task_id", schema: "project", table: "project_subtasks", column: "task_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_statuses_projects_project_id", schema: "project", table: "statuses", column: "project_id", principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_suppliers_people_person_id", schema: "basic", table: "suppliers", column: "person_id", principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_task_assignees_tasks_task_id", schema: "project", table: "task_assignees", column: "task_id", principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_projects_project_id", schema: "project", table: "tasks", column: "project_id", principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_statuses_status_id", schema: "project", table: "tasks", column: "status_id", principalSchema: "project", principalTable: "statuses", principalColumn: "id", onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey("fk_tasks_users_user_id", schema: "project", table: "tasks", column: "user_id", principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
    }
}

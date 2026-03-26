using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class Addons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_users_user_model_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_people_states_state_id",
                schema: "basic",
                table: "people");

            migrationBuilder.DropForeignKey(
                name: "fk_task_assignees_users_user_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_users_user_model_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "submodules",
                schema: "auth");

            migrationBuilder.DropIndex(
                name: "ix_task_assignees_user_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropIndex(
                name: "ix_comments_user_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "user_model_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropColumn(
                name: "city",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "complement",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "number",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "street",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "zip_code",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "language",
                schema: "auth",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "time_zone",
                schema: "auth",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "user_model_id",
                schema: "project",
                table: "comments");

            migrationBuilder.RenameColumn(
                name: "user_model_id",
                schema: "project",
                table: "tasks",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_user_model_id",
                schema: "project",
                table: "tasks",
                newName: "ix_tasks_user_id");

            migrationBuilder.RenameColumn(
                name: "state_id",
                schema: "basic",
                table: "people",
                newName: "state_model_id");

            migrationBuilder.RenameIndex(
                name: "ix_people_state_id",
                schema: "basic",
                table: "people",
                newName: "ix_people_state_model_id");

            migrationBuilder.RenameColumn(
                name: "user_model_id",
                schema: "project",
                table: "attachments",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_attachments_user_model_id",
                schema: "project",
                table: "attachments",
                newName: "ix_attachments_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "tasks",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "project",
                table: "tasks",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "project",
                table: "statuses",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "color",
                schema: "project",
                table: "statuses",
                type: "character varying(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "projects",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "project",
                table: "projects",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "project_subtasks",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "barcode",
                schema: "basic",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "basic",
                table: "products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "dimensions",
                schema: "basic",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_url",
                schema: "basic",
                table: "products",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "basic",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "max_stock_level",
                schema: "basic",
                table: "products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "min_stock_level",
                schema: "basic",
                table: "products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sku",
                schema: "basic",
                table: "products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "unit_of_measure",
                schema: "basic",
                table: "products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "weight",
                schema: "basic",
                table: "products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "date_of_birth",
                schema: "basic",
                table: "people",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "basic",
                table: "people",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "photo_url",
                schema: "basic",
                table: "people",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                schema: "basic",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "basic",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_number",
                schema: "basic",
                table: "orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "payment_method",
                schema: "basic",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_quantity",
                schema: "basic",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                schema: "auth",
                table: "orders",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                schema: "auth",
                table: "orders",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "order_number",
                schema: "auth",
                table: "orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "payment_method",
                schema: "auth",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_quantity",
                schema: "auth",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                schema: "basic",
                table: "order_details",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "subtotal",
                schema: "basic",
                table: "order_details",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "discount_amount",
                schema: "auth",
                table: "order_details",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "subtotal",
                schema: "auth",
                table: "order_details",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "auth",
                table: "modules",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "icon",
                schema: "auth",
                table: "modules",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                schema: "auth",
                table: "modules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "sort_order",
                schema: "auth",
                table: "modules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ip_address",
                schema: "auth",
                table: "forgotten_passwords",
                type: "character varying(45)",
                maxLength: 45,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "user_agent",
                schema: "auth",
                table: "forgotten_passwords",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "content",
                schema: "project",
                table: "comments",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_url",
                schema: "project",
                table: "attachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                schema: "project",
                table: "attachments",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                schema: "project",
                table: "attachments",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "number",
                schema: "auth",
                table: "addresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "complement",
                schema: "auth",
                table: "addresses",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "address_type",
                schema: "auth",
                table: "addresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "country",
                schema: "auth",
                table: "addresses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_default",
                schema: "auth",
                table: "addresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "latitude",
                schema: "auth",
                table: "addresses",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                schema: "auth",
                table: "addresses",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                schema: "auth",
                table: "addresses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "observation",
                schema: "auth",
                table: "addresses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Configuration",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    config_type = table.Column<int>(type: "integer", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configuration", x => x.id);
                    table.ForeignKey(
                        name: "fk_configuration_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "auth",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_configuration_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "auth",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "person_addresses",
                schema: "basic",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_addresses_addresses_address_id",
                        column: x => x.address_id,
                        principalSchema: "auth",
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_person_addresses_people_person_id",
                        column: x => x.person_id,
                        principalSchema: "basic",
                        principalTable: "people",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_task_assignees_user_id",
                schema: "project",
                table: "task_assignees",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                schema: "project",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuration_company_id",
                schema: "auth",
                table: "Configuration",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuration_user_id",
                schema: "auth",
                table: "Configuration",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_addresses_address_id",
                schema: "basic",
                table: "person_addresses",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_addresses_person_id",
                schema: "basic",
                table: "person_addresses",
                column: "person_id");

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
                name: "fk_comments_users_user_id",
                schema: "project",
                table: "comments",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_people_states_state_model_id",
                schema: "basic",
                table: "people",
                column: "state_model_id",
                principalSchema: "auth",
                principalTable: "states",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_assignees_users_user_id",
                schema: "project",
                table: "task_assignees",
                column: "user_id",
                principalSchema: "auth",
                principalTable: "users",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attachments_users_user_id",
                schema: "project",
                table: "attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_comments_users_user_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "fk_people_states_state_model_id",
                schema: "basic",
                table: "people");

            migrationBuilder.DropForeignKey(
                name: "fk_task_assignees_users_user_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropForeignKey(
                name: "fk_tasks_users_user_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropTable(
                name: "Configuration",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "person_addresses",
                schema: "basic");

            migrationBuilder.DropIndex(
                name: "ix_task_assignees_user_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropIndex(
                name: "ix_comments_user_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "barcode",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "dimensions",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "image_url",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "max_stock_level",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "min_stock_level",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "sku",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "unit_of_measure",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "weight",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "date_of_birth",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "photo_url",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "order_number",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "payment_method",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "total_quantity",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                schema: "auth",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "notes",
                schema: "auth",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "order_number",
                schema: "auth",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "payment_method",
                schema: "auth",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "total_quantity",
                schema: "auth",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                schema: "basic",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "subtotal",
                schema: "basic",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "discount_amount",
                schema: "auth",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "subtotal",
                schema: "auth",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "auth",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "icon",
                schema: "auth",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "is_active",
                schema: "auth",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "sort_order",
                schema: "auth",
                table: "modules");

            migrationBuilder.DropColumn(
                name: "ip_address",
                schema: "auth",
                table: "forgotten_passwords");

            migrationBuilder.DropColumn(
                name: "user_agent",
                schema: "auth",
                table: "forgotten_passwords");

            migrationBuilder.DropColumn(
                name: "address_type",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "country",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "is_default",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "latitude",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "longitude",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                schema: "auth",
                table: "addresses");

            migrationBuilder.DropColumn(
                name: "observation",
                schema: "auth",
                table: "addresses");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "project",
                table: "tasks",
                newName: "user_model_id");

            migrationBuilder.RenameIndex(
                name: "ix_tasks_user_id",
                schema: "project",
                table: "tasks",
                newName: "ix_tasks_user_model_id");

            migrationBuilder.RenameColumn(
                name: "state_model_id",
                schema: "basic",
                table: "people",
                newName: "state_id");

            migrationBuilder.RenameIndex(
                name: "ix_people_state_model_id",
                schema: "basic",
                table: "people",
                newName: "ix_people_state_id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                schema: "project",
                table: "attachments",
                newName: "user_model_id");

            migrationBuilder.RenameIndex(
                name: "ix_attachments_user_id",
                schema: "project",
                table: "attachments",
                newName: "ix_attachments_user_model_id");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "tasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "project",
                table: "tasks",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_model_id",
                schema: "project",
                table: "task_assignees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "project",
                table: "statuses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "color",
                schema: "project",
                table: "statuses",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(7)",
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "projects",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "project",
                table: "projects",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "title",
                schema: "project",
                table: "project_subtasks",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "city",
                schema: "basic",
                table: "people",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "complement",
                schema: "basic",
                table: "people",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                schema: "basic",
                table: "people",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "number",
                schema: "basic",
                table: "people",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "street",
                schema: "basic",
                table: "people",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "zip_code",
                schema: "basic",
                table: "people",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "language",
                schema: "auth",
                table: "companies",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "time_zone",
                schema: "auth",
                table: "companies",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                schema: "project",
                table: "comments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AddColumn<Guid>(
                name: "user_model_id",
                schema: "project",
                table: "comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "file_url",
                schema: "project",
                table: "attachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                schema: "project",
                table: "attachments",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "content_type",
                schema: "project",
                table: "attachments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "number",
                schema: "auth",
                table: "addresses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "complement",
                schema: "auth",
                table: "addresses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "submodules",
                schema: "auth",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    module_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    route = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_submodules", x => x.id);
                    table.ForeignKey(
                        name: "fk_submodules_modules_module_id",
                        column: x => x.module_id,
                        principalSchema: "auth",
                        principalTable: "modules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_task_assignees_user_model_id",
                schema: "project",
                table: "task_assignees",
                column: "user_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_model_id",
                schema: "project",
                table: "comments",
                column: "user_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_submodules_module_id",
                schema: "auth",
                table: "submodules",
                column: "module_id");

            migrationBuilder.AddForeignKey(
                name: "fk_attachments_users_user_model_id",
                schema: "project",
                table: "attachments",
                column: "user_model_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_model_id",
                schema: "project",
                table: "comments",
                column: "user_model_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_people_states_state_id",
                schema: "basic",
                table: "people",
                column: "state_id",
                principalSchema: "auth",
                principalTable: "states",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_task_assignees_users_user_model_id",
                schema: "project",
                table: "task_assignees",
                column: "user_model_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tasks_users_user_model_id",
                schema: "project",
                table: "tasks",
                column: "user_model_id",
                principalSchema: "auth",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class CompanyTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "task_assignees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "suppliers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "stock_movements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "statuses",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "projects",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "project_subtasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "product_categories",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "positions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "people",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "order_details",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "social_network",
                table: "followers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "social_network",
                table: "feeds",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "employees",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "basic",
                table: "customers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                schema: "project",
                table: "attachments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "task_assignees");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "statuses");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "projects");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "project_subtasks");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "products");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "people");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "social_network",
                table: "followers");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "social_network",
                table: "feeds");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "basic",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "company_id",
                schema: "project",
                table: "attachments");
        }
    }
}

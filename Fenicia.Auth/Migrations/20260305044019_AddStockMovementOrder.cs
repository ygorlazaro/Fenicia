using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "employee_id",
                schema: "basic",
                table: "stock_movements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "order_id",
                schema: "basic",
                table: "stock_movements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                schema: "basic",
                table: "stock_movements",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "employee_id",
                schema: "basic",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_employee_id",
                schema: "basic",
                table: "stock_movements",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_order_id",
                schema: "basic",
                table: "stock_movements",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_employee_id",
                schema: "basic",
                table: "orders",
                column: "employee_id");

            migrationBuilder.AddForeignKey(
                name: "fk_orders_employees_employee_id",
                schema: "basic",
                table: "orders",
                column: "employee_id",
                principalSchema: "basic",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_employees_employee_id",
                schema: "basic",
                table: "stock_movements",
                column: "employee_id",
                principalSchema: "basic",
                principalTable: "employees",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_orders_order_id",
                schema: "basic",
                table: "stock_movements",
                column: "order_id",
                principalSchema: "basic",
                principalTable: "orders",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_employees_employee_id",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_employees_employee_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_orders_order_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "ix_stock_movements_employee_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "ix_stock_movements_order_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "ix_orders_employee_id",
                schema: "basic",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "employee_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "order_id",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "reason",
                schema: "basic",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "employee_id",
                schema: "basic",
                table: "orders");
        }
    }
}

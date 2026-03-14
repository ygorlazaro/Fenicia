#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class AddStockMovementOrder : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>("employee_id", schema: "basic", table: "stock_movements", type: "uuid", nullable: true);

        migrationBuilder.AddColumn<Guid>("order_id", schema: "basic", table: "stock_movements", type: "uuid", nullable: true);

        migrationBuilder.AddColumn<string>("reason", schema: "basic", table: "stock_movements", type: "character varying(255)", maxLength: 255, nullable: true);

        migrationBuilder.AddColumn<Guid>("employee_id", schema: "basic", table: "orders", type: "uuid", nullable: true);

        migrationBuilder.CreateIndex("ix_stock_movements_employee_id", schema: "basic", table: "stock_movements", column: "employee_id");

        migrationBuilder.CreateIndex("ix_stock_movements_order_id", schema: "basic", table: "stock_movements", column: "order_id");

        migrationBuilder.CreateIndex("ix_orders_employee_id", schema: "basic", table: "orders", column: "employee_id");

        migrationBuilder.AddForeignKey("fk_orders_employees_employee_id", schema: "basic", table: "orders", column: "employee_id", principalSchema: "basic", principalTable: "employees", principalColumn: "id");

        migrationBuilder.AddForeignKey("fk_stock_movements_employees_employee_id", schema: "basic", table: "stock_movements", column: "employee_id", principalSchema: "basic", principalTable: "employees", principalColumn: "id");

        migrationBuilder.AddForeignKey("fk_stock_movements_orders_order_id", schema: "basic", table: "stock_movements", column: "order_id", principalSchema: "basic", principalTable: "orders", principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("fk_orders_employees_employee_id", schema: "basic", table: "orders");

        migrationBuilder.DropForeignKey("fk_stock_movements_employees_employee_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropForeignKey("fk_stock_movements_orders_order_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropIndex("ix_stock_movements_employee_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropIndex("ix_stock_movements_order_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropIndex("ix_orders_employee_id", schema: "basic", table: "orders");

        migrationBuilder.DropColumn("employee_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropColumn("order_id", schema: "basic", table: "stock_movements");

        migrationBuilder.DropColumn("reason", schema: "basic", table: "stock_movements");

        migrationBuilder.DropColumn("employee_id", schema: "basic", table: "orders");
    }
}
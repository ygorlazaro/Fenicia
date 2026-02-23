#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Module.Basic.Migrations;

/// <inheritdoc />
public partial class StockMovement : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "fk_customers_addresses_address_id",
            "customers");

        migrationBuilder.DropForeignKey(
            "fk_employees_addresses_address_id",
            "employees");

        migrationBuilder.DropForeignKey(
            "fk_suppliers_addresses_address_id",
            "suppliers");

        migrationBuilder.DropTable(
            "addresses");

        migrationBuilder.RenameColumn(
            "address_id",
            "suppliers",
            "state_id");

        migrationBuilder.RenameIndex(
            "ix_suppliers_address_id",
            table: "suppliers",
            newName: "ix_suppliers_state_id");

        migrationBuilder.RenameColumn(
            "address_id",
            "employees",
            "state_id");

        migrationBuilder.RenameIndex(
            "ix_employees_address_id",
            table: "employees",
            newName: "ix_employees_state_id");

        migrationBuilder.RenameColumn(
            "address_id",
            "customers",
            "state_id");

        migrationBuilder.RenameIndex(
            "ix_customers_address_id",
            table: "customers",
            newName: "ix_customers_state_id");

        migrationBuilder.AddColumn<string>(
            "city",
            "suppliers",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "complement",
            "suppliers",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "neighborhood",
            "suppliers",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "number",
            "suppliers",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "street",
            "suppliers",
            "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "zip_code",
            "suppliers",
            "character varying(9)",
            maxLength: 9,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AlterColumn<double>(
            "quantity",
            "stock_movements",
            "double precision",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AddColumn<Guid>(
            "customer_id",
            "stock_movements",
            "uuid",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            "supplier_id",
            "stock_movements",
            "uuid",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            "type",
            "stock_movements",
            "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AlterColumn<double>(
            "quantity",
            "products",
            "double precision",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AddColumn<string>(
            "city",
            "employees",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "complement",
            "employees",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "neighborhood",
            "employees",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "number",
            "employees",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "street",
            "employees",
            "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "zip_code",
            "employees",
            "character varying(9)",
            maxLength: 9,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "city",
            "customers",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "complement",
            "customers",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "neighborhood",
            "customers",
            "character varying(50)",
            maxLength: 50,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "number",
            "customers",
            "character varying(10)",
            maxLength: 10,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "street",
            "customers",
            "character varying(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            "zip_code",
            "customers",
            "character varying(9)",
            maxLength: 9,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            "orders",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                customer_id = table.Column<Guid>("uuid", nullable: false),
                total_amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                sale_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                status = table.Column<int>("integer", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_orders", x => x.id);
                table.ForeignKey(
                    "fk_orders_customers_customer_id",
                    x => x.customer_id,
                    "customers",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "order_details",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_id = table.Column<Guid>("uuid", nullable: false),
                product_id = table.Column<Guid>("uuid", nullable: false),
                price = table.Column<decimal>("numeric(18,2)", nullable: false),
                quantity = table.Column<double>("double precision", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_order_details", x => x.id);
                table.ForeignKey(
                    "fk_order_details_orders_order_id",
                    x => x.order_id,
                    "orders",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_order_details_products_product_id",
                    x => x.product_id,
                    "products",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_stock_movements_customer_id",
            "stock_movements",
            "customer_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_supplier_id",
            "stock_movements",
            "supplier_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_order_id",
            "order_details",
            "order_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_product_id",
            "order_details",
            "product_id");

        migrationBuilder.CreateIndex(
            "ix_orders_customer_id",
            "orders",
            "customer_id");

        migrationBuilder.AddForeignKey(
            "fk_customers_states_state_id",
            "customers",
            "state_id",
            "states",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_employees_states_state_id",
            "employees",
            "state_id",
            "states",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_stock_movements_customers_customer_id",
            "stock_movements",
            "customer_id",
            "customers",
            principalColumn: "id");

        migrationBuilder.AddForeignKey(
            "fk_stock_movements_suppliers_supplier_id",
            "stock_movements",
            "supplier_id",
            "suppliers",
            principalColumn: "id");

        migrationBuilder.AddForeignKey(
            "fk_suppliers_states_state_id",
            "suppliers",
            "state_id",
            "states",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "fk_customers_states_state_id",
            "customers");

        migrationBuilder.DropForeignKey(
            "fk_employees_states_state_id",
            "employees");

        migrationBuilder.DropForeignKey(
            "fk_stock_movements_customers_customer_id",
            "stock_movements");

        migrationBuilder.DropForeignKey(
            "fk_stock_movements_suppliers_supplier_id",
            "stock_movements");

        migrationBuilder.DropForeignKey(
            "fk_suppliers_states_state_id",
            "suppliers");

        migrationBuilder.DropTable(
            "order_details");

        migrationBuilder.DropTable(
            "orders");

        migrationBuilder.DropIndex(
            "ix_stock_movements_customer_id",
            "stock_movements");

        migrationBuilder.DropIndex(
            "ix_stock_movements_supplier_id",
            "stock_movements");

        migrationBuilder.DropColumn(
            "city",
            "suppliers");

        migrationBuilder.DropColumn(
            "complement",
            "suppliers");

        migrationBuilder.DropColumn(
            "neighborhood",
            "suppliers");

        migrationBuilder.DropColumn(
            "number",
            "suppliers");

        migrationBuilder.DropColumn(
            "street",
            "suppliers");

        migrationBuilder.DropColumn(
            "zip_code",
            "suppliers");

        migrationBuilder.DropColumn(
            "customer_id",
            "stock_movements");

        migrationBuilder.DropColumn(
            "supplier_id",
            "stock_movements");

        migrationBuilder.DropColumn(
            "type",
            "stock_movements");

        migrationBuilder.DropColumn(
            "city",
            "employees");

        migrationBuilder.DropColumn(
            "complement",
            "employees");

        migrationBuilder.DropColumn(
            "neighborhood",
            "employees");

        migrationBuilder.DropColumn(
            "number",
            "employees");

        migrationBuilder.DropColumn(
            "street",
            "employees");

        migrationBuilder.DropColumn(
            "zip_code",
            "employees");

        migrationBuilder.DropColumn(
            "city",
            "customers");

        migrationBuilder.DropColumn(
            "complement",
            "customers");

        migrationBuilder.DropColumn(
            "neighborhood",
            "customers");

        migrationBuilder.DropColumn(
            "number",
            "customers");

        migrationBuilder.DropColumn(
            "street",
            "customers");

        migrationBuilder.DropColumn(
            "zip_code",
            "customers");

        migrationBuilder.RenameColumn(
            "state_id",
            "suppliers",
            "address_id");

        migrationBuilder.RenameIndex(
            "ix_suppliers_state_id",
            table: "suppliers",
            newName: "ix_suppliers_address_id");

        migrationBuilder.RenameColumn(
            "state_id",
            "employees",
            "address_id");

        migrationBuilder.RenameIndex(
            "ix_employees_state_id",
            table: "employees",
            newName: "ix_employees_address_id");

        migrationBuilder.RenameColumn(
            "state_id",
            "customers",
            "address_id");

        migrationBuilder.RenameIndex(
            "ix_customers_state_id",
            table: "customers",
            newName: "ix_customers_address_id");

        migrationBuilder.AlterColumn<int>(
            "quantity",
            "stock_movements",
            "integer",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "double precision");

        migrationBuilder.AlterColumn<int>(
            "quantity",
            "products",
            "integer",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "double precision");

        migrationBuilder.CreateTable(
            "addresses",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                state_id = table.Column<Guid>("uuid", nullable: false),
                city = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                complement = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                number = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                street = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                zip_code = table.Column<string>("character varying(9)", maxLength: 9, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_addresses", x => x.id);
                table.ForeignKey(
                    "fk_addresses_states_state_id",
                    x => x.state_id,
                    "states",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_addresses_state_id",
            "addresses",
            "state_id");

        migrationBuilder.AddForeignKey(
            "fk_customers_addresses_address_id",
            "customers",
            "address_id",
            "addresses",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_employees_addresses_address_id",
            "employees",
            "address_id",
            "addresses",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_suppliers_addresses_address_id",
            "suppliers",
            "address_id",
            "addresses",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
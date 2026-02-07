#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Module.Basic.Migrations
{
    /// <inheritdoc />
    public partial class StockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_addresses_address_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_addresses_address_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_addresses_address_id",
                table: "suppliers");

            migrationBuilder.DropTable(
                name: "addresses");

            migrationBuilder.RenameColumn(
                name: "address_id",
                table: "suppliers",
                newName: "state_id");

            migrationBuilder.RenameIndex(
                name: "ix_suppliers_address_id",
                table: "suppliers",
                newName: "ix_suppliers_state_id");

            migrationBuilder.RenameColumn(
                name: "address_id",
                table: "employees",
                newName: "state_id");

            migrationBuilder.RenameIndex(
                name: "ix_employees_address_id",
                table: "employees",
                newName: "ix_employees_state_id");

            migrationBuilder.RenameColumn(
                name: "address_id",
                table: "customers",
                newName: "state_id");

            migrationBuilder.RenameIndex(
                name: "ix_customers_address_id",
                table: "customers",
                newName: "ix_customers_state_id");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "suppliers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "suppliers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                table: "suppliers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "suppliers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "suppliers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "zip_code",
                table: "suppliers",
                type: "character varying(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<double>(
                name: "quantity",
                table: "stock_movements",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "customer_id",
                table: "stock_movements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "supplier_id",
                table: "stock_movements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "stock_movements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "quantity",
                table: "products",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "employees",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "employees",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "employees",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "zip_code",
                table: "employees",
                type: "character varying(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "city",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "complement",
                table: "customers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "neighborhood",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "number",
                table: "customers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "street",
                table: "customers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "zip_code",
                table: "customers",
                type: "character varying(9)",
                maxLength: 9,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    sale_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    quantity = table.Column<double>(type: "double precision", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_details", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_details_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_order_details_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_customer_id",
                table: "stock_movements",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_stock_movements_supplier_id",
                table: "stock_movements",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_details_order_id",
                table: "order_details",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_details_product_id",
                table: "order_details",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_customer_id",
                table: "orders",
                column: "customer_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_states_state_id",
                table: "customers",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_states_state_id",
                table: "employees",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_customers_customer_id",
                table: "stock_movements",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_stock_movements_suppliers_supplier_id",
                table: "stock_movements",
                column: "supplier_id",
                principalTable: "suppliers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_states_state_id",
                table: "suppliers",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_states_state_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_states_state_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_customers_customer_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_stock_movements_suppliers_supplier_id",
                table: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_states_state_id",
                table: "suppliers");

            migrationBuilder.DropTable(
                name: "order_details");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropIndex(
                name: "ix_stock_movements_customer_id",
                table: "stock_movements");

            migrationBuilder.DropIndex(
                name: "ix_stock_movements_supplier_id",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "city",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "number",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "street",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "zip_code",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "customer_id",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "supplier_id",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "type",
                table: "stock_movements");

            migrationBuilder.DropColumn(
                name: "city",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "number",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "street",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "zip_code",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "city",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "neighborhood",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "number",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "street",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "zip_code",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "state_id",
                table: "suppliers",
                newName: "address_id");

            migrationBuilder.RenameIndex(
                name: "ix_suppliers_state_id",
                table: "suppliers",
                newName: "ix_suppliers_address_id");

            migrationBuilder.RenameColumn(
                name: "state_id",
                table: "employees",
                newName: "address_id");

            migrationBuilder.RenameIndex(
                name: "ix_employees_state_id",
                table: "employees",
                newName: "ix_employees_address_id");

            migrationBuilder.RenameColumn(
                name: "state_id",
                table: "customers",
                newName: "address_id");

            migrationBuilder.RenameIndex(
                name: "ix_customers_state_id",
                table: "customers",
                newName: "ix_customers_address_id");

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "stock_movements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    complement = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    zip_code = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_addresses_state_id",
                table: "addresses",
                column: "state_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_addresses_address_id",
                table: "customers",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_addresses_address_id",
                table: "employees",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_addresses_address_id",
                table: "suppliers",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
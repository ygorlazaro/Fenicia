#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Module.Basic.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "positions",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_positions", x => x.id); });

        migrationBuilder.CreateTable(
            "product_categories",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_product_categories", x => x.id); });

        migrationBuilder.CreateTable(
            "states",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                uf = table.Column<string>("character varying(2)", maxLength: 2, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_states", x => x.id); });

        migrationBuilder.CreateTable(
            "products",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cost_price = table.Column<decimal>("numeric", nullable: false),
                selling_price = table.Column<decimal>("numeric", nullable: false),
                quantity = table.Column<int>("integer", nullable: false),
                category_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
                table.ForeignKey(
                    "fk_products_product_categories_category_id",
                    x => x.category_id,
                    "product_categories",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "addresses",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                street = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                number = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                complement = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                zip_code = table.Column<string>("character varying(9)", maxLength: 9, nullable: false),
                state_id = table.Column<Guid>("uuid", nullable: false),
                city = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
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

        migrationBuilder.CreateTable(
            "stock_movements",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                product_id = table.Column<Guid>("uuid", nullable: false),
                quantity = table.Column<int>("integer", nullable: false),
                date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                price = table.Column<decimal>("numeric", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stock_movements", x => x.id);
                table.ForeignKey(
                    "fk_stock_movements_products_product_id",
                    x => x.product_id,
                    "products",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "customers",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cpf = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                address_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_customers", x => x.id);
                table.ForeignKey(
                    "fk_customers_addresses_address_id",
                    x => x.address_id,
                    "addresses",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "employees",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cpf = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                address_id = table.Column<Guid>("uuid", nullable: false),
                position_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_employees", x => x.id);
                table.ForeignKey(
                    "fk_employees_addresses_address_id",
                    x => x.address_id,
                    "addresses",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_employees_positions_position_id",
                    x => x.position_id,
                    "positions",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "suppliers",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cpf = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                address_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_suppliers", x => x.id);
                table.ForeignKey(
                    "fk_suppliers_addresses_address_id",
                    x => x.address_id,
                    "addresses",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_addresses_state_id",
            "addresses",
            "state_id");

        migrationBuilder.CreateIndex(
            "ix_customers_address_id",
            "customers",
            "address_id");

        migrationBuilder.CreateIndex(
            "ix_employees_address_id",
            "employees",
            "address_id");

        migrationBuilder.CreateIndex(
            "ix_employees_position_id",
            "employees",
            "position_id");

        migrationBuilder.CreateIndex(
            "ix_products_category_id",
            "products",
            "category_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_product_id",
            "stock_movements",
            "product_id");

        migrationBuilder.CreateIndex(
            "ix_suppliers_address_id",
            "suppliers",
            "address_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "customers");

        migrationBuilder.DropTable(
            "employees");

        migrationBuilder.DropTable(
            "stock_movements");

        migrationBuilder.DropTable(
            "suppliers");

        migrationBuilder.DropTable(
            "positions");

        migrationBuilder.DropTable(
            "products");

        migrationBuilder.DropTable(
            "addresses");

        migrationBuilder.DropTable(
            "product_categories");

        migrationBuilder.DropTable(
            "states");
    }
}
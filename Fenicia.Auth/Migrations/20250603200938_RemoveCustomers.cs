using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders"
            );

            migrationBuilder.DropTable(name: "customers");

            migrationBuilder.RenameColumn(name: "customer_id", table: "orders", newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_customer_id",
                table: "orders",
                newName: "ix_orders_user_id"
            );

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(
                name: "ix_orders_company_id",
                table: "orders",
                column: "company_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_orders_companies_company_id",
                table: "orders",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "fk_orders_users_user_id",
                table: "orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_orders_companies_company_id",
                table: "orders"
            );

            migrationBuilder.DropForeignKey(name: "fk_orders_users_user_id", table: "orders");

            migrationBuilder.DropIndex(name: "ix_orders_company_id", table: "orders");

            migrationBuilder.DropColumn(name: "company_id", table: "orders");

            migrationBuilder.RenameColumn(name: "user_id", table: "orders", newName: "customer_id");

            migrationBuilder.RenameIndex(
                name: "ix_orders_user_id",
                table: "orders",
                newName: "ix_orders_customer_id"
            );

            migrationBuilder.CreateTable(
                name: "customers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    deleted = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                    table.ForeignKey(
                        name: "fk_customers_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "fk_customers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_customers_company_id",
                table: "customers",
                column: "company_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_customers_user_id",
                table: "customers",
                column: "user_id",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "fk_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}

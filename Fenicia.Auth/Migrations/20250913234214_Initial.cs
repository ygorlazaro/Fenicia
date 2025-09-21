#nullable disable

namespace Fenicia.Auth.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "modules",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_modules", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "roles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "states",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_states", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "users",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                password = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                name = table.Column<string>(type: "character varying(48)", maxLength: 48, nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            name: "addresses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                complement = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                zip_code = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                state_id = table.Column<Guid>(type: "uuid", nullable: false),
                city = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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

        migrationBuilder.CreateTable(
            name: "forgotten_passwords",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_forgotten_passwords", x => x.id);
                table.ForeignKey(
                    name: "fk_forgotten_passwords_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "refresh_tokens",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                token = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    name: "fk_refresh_tokens_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "companies",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                logo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                time_zone = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                address_id = table.Column<Guid>(type: "uuid", nullable: true),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_companies", x => x.id);
                table.ForeignKey(
                    name: "fk_companies_addresses_address_id",
                    column: x => x.address_id,
                    principalTable: "addresses",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            name: "orders",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                company_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    name: "fk_orders_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_orders_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "users_roles",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                role_id = table.Column<Guid>(type: "uuid", nullable: false),
                company_id = table.Column<Guid>(type: "uuid", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users_roles", x => x.id);
                table.ForeignKey(
                    name: "fk_users_roles_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_users_roles_roles_role_id",
                    column: x => x.role_id,
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_users_roles_users_user_id",
                    column: x => x.user_id,
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "order_details",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                order_id = table.Column<Guid>(type: "uuid", nullable: false),
                module_id = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_order_details", x => x.id);
                table.ForeignKey(
                    name: "fk_order_details_modules_module_id",
                    column: x => x.module_id,
                    principalTable: "modules",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_order_details_orders_order_id",
                    column: x => x.order_id,
                    principalTable: "orders",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "subscriptions",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                company_id = table.Column<Guid>(type: "uuid", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                order_id = table.Column<Guid>(type: "uuid", nullable: true),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscriptions", x => x.id);
                table.ForeignKey(
                    name: "fk_subscriptions_companies_company_id",
                    column: x => x.company_id,
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_subscriptions_orders_order_id",
                    column: x => x.order_id,
                    principalTable: "orders",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            name: "subscription_credits",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                module_id = table.Column<Guid>(type: "uuid", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                order_detail_id = table.Column<Guid>(type: "uuid", nullable: true),
                created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_credits", x => x.id);
                table.ForeignKey(
                    name: "fk_subscription_credits_modules_module_id",
                    column: x => x.module_id,
                    principalTable: "modules",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_subscription_credits_order_details_order_detail_id",
                    column: x => x.order_detail_id,
                    principalTable: "order_details",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "fk_subscription_credits_subscriptions_subscription_id",
                    column: x => x.subscription_id,
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "ix_addresses_state_id",
            table: "addresses",
            column: "state_id");

        migrationBuilder.CreateIndex(
            name: "ix_companies_address_id",
            table: "companies",
            column: "address_id");

        migrationBuilder.CreateIndex(
            name: "ix_forgotten_passwords_user_id",
            table: "forgotten_passwords",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_order_details_module_id",
            table: "order_details",
            column: "module_id");

        migrationBuilder.CreateIndex(
            name: "ix_order_details_order_id",
            table: "order_details",
            column: "order_id");

        migrationBuilder.CreateIndex(
            name: "ix_orders_company_id",
            table: "orders",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "ix_orders_user_id",
            table: "orders",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_refresh_tokens_user_id",
            table: "refresh_tokens",
            column: "user_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_credits_module_id",
            table: "subscription_credits",
            column: "module_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscription_credits_order_detail_id",
            table: "subscription_credits",
            column: "order_detail_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_subscription_credits_subscription_id",
            table: "subscription_credits",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscriptions_company_id",
            table: "subscriptions",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "ix_subscriptions_order_id",
            table: "subscriptions",
            column: "order_id",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_users_roles_company_id",
            table: "users_roles",
            column: "company_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_roles_role_id",
            table: "users_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            name: "ix_users_roles_user_id",
            table: "users_roles",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "forgotten_passwords");

        migrationBuilder.DropTable(
            name: "refresh_tokens");

        migrationBuilder.DropTable(
            name: "subscription_credits");

        migrationBuilder.DropTable(
            name: "users_roles");

        migrationBuilder.DropTable(
            name: "order_details");

        migrationBuilder.DropTable(
            name: "subscriptions");

        migrationBuilder.DropTable(
            name: "roles");

        migrationBuilder.DropTable(
            name: "modules");

        migrationBuilder.DropTable(
            name: "orders");

        migrationBuilder.DropTable(
            name: "companies");

        migrationBuilder.DropTable(
            name: "users");

        migrationBuilder.DropTable(
            name: "addresses");

        migrationBuilder.DropTable(
            name: "states");
    }
}

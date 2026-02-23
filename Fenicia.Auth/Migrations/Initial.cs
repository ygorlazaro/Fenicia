#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "modules",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                amount = table.Column<decimal>("numeric(18,2)", precision: 18, scale: 2, nullable: false),
                type = table.Column<int>("integer", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_modules", x => x.id); });

        migrationBuilder.CreateTable(
            "roles",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_roles", x => x.id); });

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
            "users",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                email = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                password = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
                name = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_users", x => x.id); });

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
            "forgotten_passwords",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                code = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                expiration_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_forgotten_passwords", x => x.id);
                table.ForeignKey(
                    "fk_forgotten_passwords_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "refresh_tokens",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                token = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                expiration_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_refresh_tokens", x => x.id);
                table.ForeignKey(
                    "fk_refresh_tokens_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "companies",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cnpj = table.Column<string>("character varying(14)", maxLength: 14, nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                logo = table.Column<string>("character varying(32)", maxLength: 32, nullable: true),
                time_zone = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                language = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                address_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_companies", x => x.id);
                table.ForeignKey(
                    "fk_companies_addresses_address_id",
                    x => x.address_id,
                    "addresses",
                    "id");
            });

        migrationBuilder.CreateTable(
            "orders",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                company_id = table.Column<Guid>("uuid", nullable: false),
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
                    "fk_orders_companies_company_id",
                    x => x.company_id,
                    "companies",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_orders_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "users_roles",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                role_id = table.Column<Guid>("uuid", nullable: false),
                company_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users_roles", x => x.id);
                table.ForeignKey(
                    "fk_users_roles_companies_company_id",
                    x => x.company_id,
                    "companies",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_users_roles_roles_role_id",
                    x => x.role_id,
                    "roles",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_users_roles_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "order_details",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_id = table.Column<Guid>("uuid", nullable: false),
                module_id = table.Column<Guid>("uuid", nullable: false),
                amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_order_details", x => x.id);
                table.ForeignKey(
                    "fk_order_details_modules_module_id",
                    x => x.module_id,
                    "modules",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_order_details_orders_order_id",
                    x => x.order_id,
                    "orders",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "subscriptions",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                status = table.Column<int>("integer", nullable: false),
                company_id = table.Column<Guid>("uuid", nullable: false),
                start_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                order_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscriptions", x => x.id);
                table.ForeignKey(
                    "fk_subscriptions_companies_company_id",
                    x => x.company_id,
                    "companies",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_subscriptions_orders_order_id",
                    x => x.order_id,
                    "orders",
                    "id");
            });

        migrationBuilder.CreateTable(
            "subscription_credits",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                subscription_id = table.Column<Guid>("uuid", nullable: false),
                module_id = table.Column<Guid>("uuid", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                start_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                end_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                order_detail_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_subscription_credits", x => x.id);
                table.ForeignKey(
                    "fk_subscription_credits_modules_module_id",
                    x => x.module_id,
                    "modules",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_subscription_credits_order_details_order_detail_id",
                    x => x.order_detail_id,
                    "order_details",
                    "id");
                table.ForeignKey(
                    "fk_subscription_credits_subscriptions_subscription_id",
                    x => x.subscription_id,
                    "subscriptions",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_addresses_state_id",
            "addresses",
            "state_id");

        migrationBuilder.CreateIndex(
            "ix_companies_address_id",
            "companies",
            "address_id");

        migrationBuilder.CreateIndex(
            "ix_forgotten_passwords_user_id",
            "forgotten_passwords",
            "user_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_module_id",
            "order_details",
            "module_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_order_id",
            "order_details",
            "order_id");

        migrationBuilder.CreateIndex(
            "ix_orders_company_id",
            "orders",
            "company_id");

        migrationBuilder.CreateIndex(
            "ix_orders_user_id",
            "orders",
            "user_id");

        migrationBuilder.CreateIndex(
            "ix_refresh_tokens_user_id",
            "refresh_tokens",
            "user_id");

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_module_id",
            "subscription_credits",
            "module_id");

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_order_detail_id",
            "subscription_credits",
            "order_detail_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_subscription_id",
            "subscription_credits",
            "subscription_id");

        migrationBuilder.CreateIndex(
            "ix_subscriptions_company_id",
            "subscriptions",
            "company_id");

        migrationBuilder.CreateIndex(
            "ix_subscriptions_order_id",
            "subscriptions",
            "order_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_users_roles_company_id",
            "users_roles",
            "company_id");

        migrationBuilder.CreateIndex(
            "ix_users_roles_role_id",
            "users_roles",
            "role_id");

        migrationBuilder.CreateIndex(
            "ix_users_roles_user_id",
            "users_roles",
            "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "forgotten_passwords");

        migrationBuilder.DropTable(
            "refresh_tokens");

        migrationBuilder.DropTable(
            "subscription_credits");

        migrationBuilder.DropTable(
            "users_roles");

        migrationBuilder.DropTable(
            "order_details");

        migrationBuilder.DropTable(
            "subscriptions");

        migrationBuilder.DropTable(
            "roles");

        migrationBuilder.DropTable(
            "modules");

        migrationBuilder.DropTable(
            "orders");

        migrationBuilder.DropTable(
            "companies");

        migrationBuilder.DropTable(
            "users");

        migrationBuilder.DropTable(
            "addresses");

        migrationBuilder.DropTable(
            "states");
    }
}
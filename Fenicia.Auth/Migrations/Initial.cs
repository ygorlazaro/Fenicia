#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema("auth");

        migrationBuilder.EnsureSchema("project");

        migrationBuilder.EnsureSchema("basic");

        migrationBuilder.EnsureSchema("social_network");

        migrationBuilder.CreateTable("modules", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
            price = table.Column<decimal>("numeric(18,2)", precision: 18, scale: 2, nullable: false),
            type = table.Column<int>("integer", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_modules", x => x.id); });

        migrationBuilder.CreateTable("positions", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_positions", x => x.id); });

        migrationBuilder.CreateTable("product_categories", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_product_categories", x => x.id); });

        migrationBuilder.CreateTable("projects", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            title = table.Column<string>("text", nullable: false),
            description = table.Column<string>("text", nullable: true),
            status = table.Column<int>("integer", nullable: false),
            start_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
            end_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
            owner = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_projects", x => x.id); });

        migrationBuilder.CreateTable("roles", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_roles", x => x.id); });

        migrationBuilder.CreateTable("states", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
            uf = table.Column<string>("character varying(2)", maxLength: 2, nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_states", x => x.id); });

        migrationBuilder.CreateTable("users", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            email = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
            password = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
            name = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
            image_url = table.Column<string>("character varying(48)", maxLength: 48, nullable: true),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table => { table.PrimaryKey("pk_users", x => x.id); });

        migrationBuilder.CreateTable("submodules", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            route = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            description = table.Column<string>("character varying(100)", maxLength: 100, nullable: true),
            module_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_submodules", x => x.id);
            table.ForeignKey("fk_submodules_modules_module_id", x => x.module_id, principalSchema: "auth", principalTable: "modules", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("products", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            cost_price = table.Column<decimal>("numeric", nullable: true),
            sales_price = table.Column<decimal>("numeric", nullable: false),
            quantity = table.Column<double>("double precision", nullable: false),
            category_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_products", x => x.id);
            table.ForeignKey("fk_products_product_categories_category_id", x => x.category_id, principalSchema: "basic", principalTable: "product_categories", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("statuses", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            project_id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("text", nullable: false),
            color = table.Column<string>("text", nullable: false),
            order = table.Column<int>("integer", nullable: false),
            is_final = table.Column<bool>("boolean", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_statuses", x => x.id);
            table.ForeignKey("fk_statuses_projects_project_id", x => x.project_id, principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("addresses", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            street = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            number = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            complement = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            zip_code = table.Column<string>("character varying(8)", maxLength: 8, nullable: false),
            state_id = table.Column<Guid>("uuid", nullable: false),
            city = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_addresses", x => x.id);
            table.ForeignKey("fk_addresses_states_state_id", x => x.state_id, principalSchema: "auth", principalTable: "states", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("people", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            document = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
            street = table.Column<string>("character varying(100)", maxLength: 100, nullable: true),
            number = table.Column<string>("character varying(10)", maxLength: 10, nullable: true),
            complement = table.Column<string>("character varying(10)", maxLength: 10, nullable: true),
            neighborhood = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
            zip_code = table.Column<string>("character varying(8)", maxLength: 8, nullable: true),
            state_id = table.Column<Guid>("uuid", nullable: true),
            city = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
            email = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
            phone_number = table.Column<string>("character varying(20)", maxLength: 20, nullable: true),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_people", x => x.id);
            table.ForeignKey("fk_people_states_state_id", x => x.state_id, principalSchema: "auth", principalTable: "states", principalColumn: "id");
        });

        migrationBuilder.CreateTable("feeds", schema: "social_network", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            date = table.Column<DateTime>("timestamp with time zone", nullable: false),
            text = table.Column<string>("character varying(512)", maxLength: 512, nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_feeds", x => x.id);
            table.ForeignKey("fk_feeds_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("followers", schema: "social_network", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            follower_id = table.Column<Guid>("uuid", nullable: false),
            follow_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
            is_active = table.Column<bool>("boolean", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_followers", x => x.id);
            table.ForeignKey("fk_followers_users_follower_id", x => x.follower_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_followers_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("forgotten_passwords", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            code = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
            expiration_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
            is_active = table.Column<bool>("boolean", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_forgotten_passwords", x => x.id);
            table.ForeignKey("fk_forgotten_passwords_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("tasks", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            project_id = table.Column<Guid>("uuid", nullable: false),
            status_id = table.Column<Guid>("uuid", nullable: false),
            title = table.Column<string>("text", nullable: false),
            description = table.Column<string>("text", nullable: true),
            priority = table.Column<int>("integer", nullable: false),
            type = table.Column<int>("integer", nullable: false),
            order = table.Column<int>("integer", nullable: false),
            estimate_points = table.Column<int>("integer", nullable: true),
            due_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
            created_by = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_tasks", x => x.id);
            table.ForeignKey("fk_tasks_projects_project_id", x => x.project_id, principalSchema: "project", principalTable: "projects", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_tasks_statuses_status_id", x => x.status_id, principalSchema: "project", principalTable: "statuses", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_tasks_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("companies", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
            cnpj = table.Column<string>("character varying(14)", maxLength: 14, nullable: false),
            is_active = table.Column<bool>("boolean", nullable: false),
            time_zone = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
            language = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
            address_id = table.Column<Guid>("uuid", nullable: true),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_companies", x => x.id);
            table.ForeignKey("fk_companies_addresses_address_id", x => x.address_id, principalSchema: "auth", principalTable: "addresses", principalColumn: "id");
        });

        migrationBuilder.CreateTable("customers", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            person_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_customers", x => x.id);
            table.ForeignKey("fk_customers_people_person_id", x => x.person_id, principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("employees", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            position_id = table.Column<Guid>("uuid", nullable: false),
            person_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_employees", x => x.id);
            table.ForeignKey("fk_employees_people_person_id", x => x.person_id, principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_employees_positions_position_id", x => x.position_id, principalSchema: "basic", principalTable: "positions", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("suppliers", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            cnpj = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
            person_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_suppliers", x => x.id);
            table.ForeignKey("fk_suppliers_people_person_id", x => x.person_id, principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("attachments", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            task_id = table.Column<Guid>("uuid", nullable: false),
            file_name = table.Column<string>("text", nullable: false),
            file_url = table.Column<string>("text", nullable: false),
            file_size = table.Column<long>("bigint", nullable: false),
            uploaded_by = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_attachments", x => x.id);
            table.ForeignKey("fk_attachments_tasks_task_id", x => x.task_id, principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_attachments_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("comments", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            task_id = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            content = table.Column<string>("text", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_comments", x => x.id);
            table.ForeignKey("fk_comments_tasks_task_id", x => x.task_id, principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_comments_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("project_subtasks", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            task_id = table.Column<Guid>("uuid", nullable: false),
            title = table.Column<string>("text", nullable: false),
            is_completed = table.Column<bool>("boolean", nullable: false),
            order = table.Column<int>("integer", nullable: false),
            completed_at = table.Column<DateTime>("timestamp with time zone", nullable: true),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_project_subtasks", x => x.id);
            table.ForeignKey("fk_project_subtasks_tasks_task_id", x => x.task_id, principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("task_assignees", schema: "project", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            task_id = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            role = table.Column<int>("integer", nullable: false),
            assigned_at = table.Column<DateTime>("timestamp with time zone", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_task_assignees", x => x.id);
            table.ForeignKey("fk_task_assignees_tasks_task_id", x => x.task_id, principalSchema: "project", principalTable: "tasks", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("orders", schema: "auth", columns: table => new
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
        }, constraints: table =>
        {
            table.PrimaryKey("pk_orders", x => x.id);
            table.ForeignKey("fk_orders_companies_company_id", x => x.company_id, principalSchema: "auth", principalTable: "companies", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_orders_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("users_roles", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            user_id = table.Column<Guid>("uuid", nullable: false),
            role_id = table.Column<Guid>("uuid", nullable: false),
            company_id = table.Column<Guid>("uuid", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_users_roles", x => x.id);
            table.ForeignKey("fk_users_roles_companies_company_id", x => x.company_id, principalSchema: "auth", principalTable: "companies", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_users_roles_roles_role_id", x => x.role_id, principalSchema: "auth", principalTable: "roles", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_users_roles_users_user_id", x => x.user_id, principalSchema: "auth", principalTable: "users", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("orders", schema: "basic", columns: table => new
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
        }, constraints: table =>
        {
            table.PrimaryKey("pk_orders1", x => x.id);
            table.ForeignKey("fk_orders_customers_customer_id", x => x.customer_id, principalSchema: "basic", principalTable: "customers", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("stock_movements", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            product_id = table.Column<Guid>("uuid", nullable: false),
            quantity = table.Column<double>("double precision", nullable: false),
            date = table.Column<DateTime>("timestamp with time zone", nullable: true),
            price = table.Column<decimal>("numeric", nullable: false),
            type = table.Column<int>("integer", nullable: false),
            customer_id = table.Column<Guid>("uuid", nullable: true),
            supplier_id = table.Column<Guid>("uuid", nullable: true),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_stock_movements", x => x.id);
            table.ForeignKey("fk_stock_movements_customers_customer_id", x => x.customer_id, principalSchema: "basic", principalTable: "customers", principalColumn: "id");
            table.ForeignKey("fk_stock_movements_products_product_id", x => x.product_id, principalSchema: "basic", principalTable: "products", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_stock_movements_suppliers_supplier_id", x => x.supplier_id, principalSchema: "basic", principalTable: "suppliers", principalColumn: "id");
        });

        migrationBuilder.CreateTable("order_details", schema: "auth", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            order_id = table.Column<Guid>("uuid", nullable: false),
            module_id = table.Column<Guid>("uuid", nullable: false),
            price = table.Column<decimal>("numeric(18,2)", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_order_details", x => x.id);
            table.ForeignKey("fk_order_details_modules_module_id", x => x.module_id, principalSchema: "auth", principalTable: "modules", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_order_details_orders_order_id", x => x.order_id, principalSchema: "auth", principalTable: "orders", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("subscriptions", schema: "auth", columns: table => new
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
        }, constraints: table =>
        {
            table.PrimaryKey("pk_subscriptions", x => x.id);
            table.ForeignKey("fk_subscriptions_companies_company_id", x => x.company_id, principalSchema: "auth", principalTable: "companies", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_subscriptions_orders_order_id", x => x.order_id, principalSchema: "auth", principalTable: "orders", principalColumn: "id");
        });

        migrationBuilder.CreateTable("order_details", schema: "basic", columns: table => new
        {
            id = table.Column<Guid>("uuid", nullable: false),
            order_id = table.Column<Guid>("uuid", nullable: false),
            product_id = table.Column<Guid>("uuid", nullable: false),
            price = table.Column<decimal>("numeric(18,2)", nullable: false),
            quantity = table.Column<double>("double precision", nullable: false),
            created = table.Column<DateTime>("timestamp with time zone", nullable: false),
            updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
            deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("pk_order_details1", x => x.id);
            table.ForeignKey("fk_order_details_orders_order_id", x => x.order_id, principalSchema: "basic", principalTable: "orders", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_order_details_products_product_id", x => x.product_id, principalSchema: "basic", principalTable: "products", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateTable("subscription_credits", schema: "auth", columns: table => new
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
        }, constraints: table =>
        {
            table.PrimaryKey("pk_subscription_credits", x => x.id);
            table.ForeignKey("fk_subscription_credits_modules_module_id", x => x.module_id, principalSchema: "auth", principalTable: "modules", principalColumn: "id", onDelete: ReferentialAction.Cascade);
            table.ForeignKey("fk_subscription_credits_order_details_order_detail_id", x => x.order_detail_id, principalSchema: "auth", principalTable: "order_details", principalColumn: "id");
            table.ForeignKey("fk_subscription_credits_subscriptions_subscription_id", x => x.subscription_id, principalSchema: "auth", principalTable: "subscriptions", principalColumn: "id", onDelete: ReferentialAction.Cascade);
        });

        migrationBuilder.CreateIndex("ix_addresses_state_id", schema: "auth", table: "addresses", column: "state_id");

        migrationBuilder.CreateIndex("ix_attachments_task_id", schema: "project", table: "attachments", column: "task_id");

        migrationBuilder.CreateIndex("ix_attachments_user_id", schema: "project", table: "attachments", column: "user_id");

        migrationBuilder.CreateIndex("ix_comments_task_id", schema: "project", table: "comments", column: "task_id");

        migrationBuilder.CreateIndex("ix_comments_user_id", schema: "project", table: "comments", column: "user_id");

        migrationBuilder.CreateIndex("ix_companies_address_id", schema: "auth", table: "companies", column: "address_id");

        migrationBuilder.CreateIndex("ix_customers_person_id", schema: "basic", table: "customers", column: "person_id", unique: true);

        migrationBuilder.CreateIndex("ix_employees_person_id", schema: "basic", table: "employees", column: "person_id", unique: true);

        migrationBuilder.CreateIndex("ix_employees_position_id", schema: "basic", table: "employees", column: "position_id");

        migrationBuilder.CreateIndex("ix_feeds_user_id", schema: "social_network", table: "feeds", column: "user_id");

        migrationBuilder.CreateIndex("ix_followers_follower_id", schema: "social_network", table: "followers", column: "follower_id");

        migrationBuilder.CreateIndex("ix_followers_user_id", schema: "social_network", table: "followers", column: "user_id");

        migrationBuilder.CreateIndex("ix_forgotten_passwords_user_id", schema: "auth", table: "forgotten_passwords", column: "user_id");

        migrationBuilder.CreateIndex("ix_order_details_module_id", schema: "auth", table: "order_details", column: "module_id");

        migrationBuilder.CreateIndex("ix_order_details_order_id", schema: "auth", table: "order_details", column: "order_id");

        migrationBuilder.CreateIndex("ix_order_details_order_id1", schema: "basic", table: "order_details", column: "order_id");

        migrationBuilder.CreateIndex("ix_order_details_product_id", schema: "basic", table: "order_details", column: "product_id");

        migrationBuilder.CreateIndex("ix_orders_company_id", schema: "auth", table: "orders", column: "company_id");

        migrationBuilder.CreateIndex("ix_orders_user_id", schema: "auth", table: "orders", column: "user_id");

        migrationBuilder.CreateIndex("ix_orders_customer_id", schema: "basic", table: "orders", column: "customer_id");

        migrationBuilder.CreateIndex("ix_people_state_id", schema: "basic", table: "people", column: "state_id");

        migrationBuilder.CreateIndex("ix_products_category_id", schema: "basic", table: "products", column: "category_id");

        migrationBuilder.CreateIndex("ix_project_subtasks_task_id", schema: "project", table: "project_subtasks", column: "task_id");

        migrationBuilder.CreateIndex("ix_statuses_project_id", schema: "project", table: "statuses", column: "project_id");

        migrationBuilder.CreateIndex("ix_stock_movements_customer_id", schema: "basic", table: "stock_movements", column: "customer_id");

        migrationBuilder.CreateIndex("ix_stock_movements_product_id", schema: "basic", table: "stock_movements", column: "product_id");

        migrationBuilder.CreateIndex("ix_stock_movements_supplier_id", schema: "basic", table: "stock_movements", column: "supplier_id");

        migrationBuilder.CreateIndex("ix_submodules_module_id", schema: "auth", table: "submodules", column: "module_id");

        migrationBuilder.CreateIndex("ix_subscription_credits_module_id", schema: "auth", table: "subscription_credits", column: "module_id");

        migrationBuilder.CreateIndex("ix_subscription_credits_order_detail_id", schema: "auth", table: "subscription_credits", column: "order_detail_id", unique: true);

        migrationBuilder.CreateIndex("ix_subscription_credits_subscription_id", schema: "auth", table: "subscription_credits", column: "subscription_id");

        migrationBuilder.CreateIndex("ix_subscriptions_company_id", schema: "auth", table: "subscriptions", column: "company_id");

        migrationBuilder.CreateIndex("ix_subscriptions_order_id", schema: "auth", table: "subscriptions", column: "order_id", unique: true);

        migrationBuilder.CreateIndex("ix_suppliers_person_id", schema: "basic", table: "suppliers", column: "person_id");

        migrationBuilder.CreateIndex("ix_task_assignees_task_id", schema: "project", table: "task_assignees", column: "task_id");

        migrationBuilder.CreateIndex("ix_tasks_project_id", schema: "project", table: "tasks", column: "project_id");

        migrationBuilder.CreateIndex("ix_tasks_status_id", schema: "project", table: "tasks", column: "status_id");

        migrationBuilder.CreateIndex("ix_tasks_user_id", schema: "project", table: "tasks", column: "user_id");

        migrationBuilder.CreateIndex("ix_users_roles_company_id", schema: "auth", table: "users_roles", column: "company_id");

        migrationBuilder.CreateIndex("ix_users_roles_role_id", schema: "auth", table: "users_roles", column: "role_id");

        migrationBuilder.CreateIndex("ix_users_roles_user_id", schema: "auth", table: "users_roles", column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("attachments", "project");

        migrationBuilder.DropTable("comments", "project");

        migrationBuilder.DropTable("employees", "basic");

        migrationBuilder.DropTable("feeds", "social_network");

        migrationBuilder.DropTable("followers", "social_network");

        migrationBuilder.DropTable("forgotten_passwords", "auth");

        migrationBuilder.DropTable("order_details", "basic");

        migrationBuilder.DropTable("project_subtasks", "project");

        migrationBuilder.DropTable("stock_movements", "basic");

        migrationBuilder.DropTable("submodules", "auth");

        migrationBuilder.DropTable("subscription_credits", "auth");

        migrationBuilder.DropTable("task_assignees", "project");

        migrationBuilder.DropTable("users_roles", "auth");

        migrationBuilder.DropTable("positions", "basic");

        migrationBuilder.DropTable("orders", "basic");

        migrationBuilder.DropTable("products", "basic");

        migrationBuilder.DropTable("suppliers", "basic");

        migrationBuilder.DropTable("order_details", "auth");

        migrationBuilder.DropTable("subscriptions", "auth");

        migrationBuilder.DropTable("tasks", "project");

        migrationBuilder.DropTable("roles", "auth");

        migrationBuilder.DropTable("customers", "basic");

        migrationBuilder.DropTable("product_categories", "basic");

        migrationBuilder.DropTable("modules", "auth");

        migrationBuilder.DropTable("orders", "auth");

        migrationBuilder.DropTable("statuses", "project");

        migrationBuilder.DropTable("people", "basic");

        migrationBuilder.DropTable("companies", "auth");

        migrationBuilder.DropTable("users", "auth");

        migrationBuilder.DropTable("projects", "project");

        migrationBuilder.DropTable("addresses", "auth");

        migrationBuilder.DropTable("states", "auth");
    }
}
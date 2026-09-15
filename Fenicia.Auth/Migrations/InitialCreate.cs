using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            "auth");

        migrationBuilder.EnsureSchema(
            "project");

        migrationBuilder.EnsureSchema(
            "social_network");

        migrationBuilder.EnsureSchema(
            "basic");

        migrationBuilder.CreateTable(
            "modules",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                price = table.Column<decimal>("numeric(18,2)", precision: 18, scale: 2, nullable: false),
                type = table.Column<int>("integer", nullable: false),
                description = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
                icon = table.Column<string>("character varying(100)", maxLength: 100, nullable: true),
                is_active = table.Column<bool>("boolean", nullable: false),
                sort_order = table.Column<int>("integer", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_modules", x => x.id);
            });

        migrationBuilder.CreateTable(
            "notifications",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                title = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
                date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                image_url = table.Column<string>("character varying(200)", maxLength: 200, nullable: true),
                read = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_notifications", x => x.id);
            });

        migrationBuilder.CreateTable(
            "positions",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_positions", x => x.id);
            });

        migrationBuilder.CreateTable(
            "product_categories",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_product_categories", x => x.id);
            });

        migrationBuilder.CreateTable(
            "projects",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                description = table.Column<string>("character varying(4096)", maxLength: 4096, nullable: true),
                status = table.Column<int>("integer", nullable: false),
                start_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                end_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                owner = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_projects", x => x.id);
            });

        migrationBuilder.CreateTable(
            "roles",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_roles", x => x.id);
            });

        migrationBuilder.CreateTable(
            "states",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                uf = table.Column<string>("character varying(2)", maxLength: 2, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_states", x => x.id);
            });

        migrationBuilder.CreateTable(
            "uploads",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                original_file_name = table.Column<string>("character varying(260)", maxLength: 260, nullable: false),
                stored_file_name = table.Column<string>("character varying(260)", maxLength: 260, nullable: false),
                content_type = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                size_bytes = table.Column<long>("bigint", nullable: false),
                url = table.Column<string>("character varying(260)", maxLength: 260, nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_uploads", x => x.id);
            });

        migrationBuilder.CreateTable(
            "users",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                email = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                password = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
                name = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                image_url = table.Column<string>("character varying(48)", maxLength: 48, nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_users", x => x.id);
            });

        migrationBuilder.CreateTable(
            "statuses",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                project_id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                color = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                order = table.Column<int>("integer", nullable: false),
                is_final = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_statuses", x => x.id);
                table.ForeignKey(
                    "fk_statuses_projects_project_id",
                    x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "addresses",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                street = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                number = table.Column<string>("character varying(20)", maxLength: 20, nullable: false),
                complement = table.Column<string>("character varying(20)", maxLength: 20, nullable: true),
                neighborhood = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                zip_code = table.Column<string>("character varying(8)", maxLength: 8, nullable: false),
                state_id = table.Column<Guid>("uuid", nullable: false),
                city = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                country = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                address_type = table.Column<int>("integer", nullable: false),
                latitude = table.Column<double>("double precision", nullable: true),
                longitude = table.Column<double>("double precision", nullable: true),
                is_default = table.Column<bool>("boolean", nullable: false),
                observation = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
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
                    principalSchema: "auth",
                    principalTable: "states",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "people",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                document = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                email = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                phone_number = table.Column<string>("character varying(20)", maxLength: 20, nullable: true),
                date_of_birth = table.Column<DateTime>("timestamp with time zone", nullable: true),
                state_id = table.Column<Guid>("uuid", nullable: true),
                photo_url = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
                notes = table.Column<string>("character varying(1000)", maxLength: 1000, nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_people", x => x.id);
                table.ForeignKey(
                    "fk_people_states_state_id",
                    x => x.state_id,
                    principalSchema: "auth",
                    principalTable: "states",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "forgotten_passwords",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                code = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                expiration_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                ip_address = table.Column<string>("character varying(45)", maxLength: 45, nullable: true),
                user_agent = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
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
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "profiles",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                user_name = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                bio = table.Column<string>("character varying(160)", maxLength: 160, nullable: true),
                upload_id = table.Column<Guid>("uuid", nullable: true),
                website = table.Column<string>("character varying(120)", maxLength: 120, nullable: true),
                location = table.Column<string>("character varying(64)", maxLength: 64, nullable: true),
                phone = table.Column<string>("character varying(24)", maxLength: 24, nullable: true),
                birth_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_profiles", x => x.id);
                table.ForeignKey(
                    "fk_profiles_uploads_upload_id",
                    x => x.upload_id,
                    principalSchema: "auth",
                    principalTable: "uploads",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_profiles_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "reports",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                reporter_id = table.Column<Guid>("uuid", nullable: false),
                target_id = table.Column<Guid>("uuid", nullable: false),
                target_type = table.Column<string>("character varying(32)", maxLength: 32, nullable: false),
                reason = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                description = table.Column<string>("character varying(512)", maxLength: 512, nullable: true),
                status = table.Column<int>("integer", nullable: false),
                report_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_reports", x => x.id);
                table.ForeignKey(
                    "fk_reports_users_reporter_id",
                    x => x.reporter_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "sprints",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                project_id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                start_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                end_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                description = table.Column<string>("character varying(4096)", maxLength: 4096, nullable: true),
                created_by = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_sprints", x => x.id);
                table.ForeignKey(
                    "fk_sprints_projects_project_id",
                    x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_sprints_users_created_by",
                    x => x.created_by,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "teams",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(128)", maxLength: 128, nullable: false),
                description = table.Column<string>("character varying(2000)", maxLength: 2000, nullable: true),
                color = table.Column<string>("character varying(30)", maxLength: 30, nullable: false),
                project_id = table.Column<Guid>("uuid", nullable: false),
                created_by = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_teams", x => x.id);
                table.ForeignKey(
                    "fk_teams_projects_project_id",
                    x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_teams_users_created_by",
                    x => x.created_by,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "companies",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cnpj = table.Column<string>("character varying(14)", maxLength: 14, nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
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
                    principalSchema: "auth",
                    principalTable: "addresses",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "customers",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                person_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_customers", x => x.id);
                table.ForeignKey(
                    "fk_customers_people_person_id",
                    x => x.person_id,
                    principalSchema: "basic",
                    principalTable: "people",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "employees",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                position_id = table.Column<Guid>("uuid", nullable: false),
                person_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_employees", x => x.id);
                table.ForeignKey(
                    "fk_employees_people_person_id",
                    x => x.person_id,
                    principalSchema: "basic",
                    principalTable: "people",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_employees_positions_position_id",
                    x => x.position_id,
                    principalSchema: "basic",
                    principalTable: "positions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "person_addresses",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                person_id = table.Column<Guid>("uuid", nullable: false),
                address_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_person_addresses", x => x.id);
                table.ForeignKey(
                    "fk_person_addresses_addresses_address_id",
                    x => x.address_id,
                    principalSchema: "auth",
                    principalTable: "addresses",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_person_addresses_people_person_id",
                    x => x.person_id,
                    principalSchema: "basic",
                    principalTable: "people",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "suppliers",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                cnpj = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                person_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_suppliers", x => x.id);
                table.ForeignKey(
                    "fk_suppliers_people_person_id",
                    x => x.person_id,
                    principalSchema: "basic",
                    principalTable: "people",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "blocks",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                blocked_profile_id = table.Column<Guid>("uuid", nullable: false),
                reason = table.Column<string>("character varying(256)", maxLength: 256, nullable: true),
                block_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_blocks", x => x.id);
                table.ForeignKey(
                    "fk_blocks_profiles_blocked_profile_id",
                    x => x.blocked_profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_blocks_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "feeds",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                text = table.Column<string>("character varying(512)", maxLength: 512, nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                original_feed_id = table.Column<Guid>("uuid", nullable: true),
                total_likes = table.Column<int>("integer", nullable: false),
                total_comments = table.Column<int>("integer", nullable: false),
                total_shares = table.Column<int>("integer", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_feeds", x => x.id);
                table.ForeignKey(
                    "fk_feeds_feeds_original_feed_id",
                    x => x.original_feed_id,
                    principalSchema: "social_network",
                    principalTable: "feeds",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_feeds_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "friendships",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                target_profile_id = table.Column<Guid>("uuid", nullable: false),
                follow_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_friendships", x => x.id);
                table.ForeignKey(
                    "fk_friendships_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_friendships_profiles_target_profile_id",
                    x => x.target_profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "tasks",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                project_id = table.Column<Guid>("uuid", nullable: false),
                status_id = table.Column<Guid>("uuid", nullable: false),
                title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                description = table.Column<string>("character varying(4096)", maxLength: 4096, nullable: true),
                priority = table.Column<int>("integer", nullable: false),
                type = table.Column<int>("integer", nullable: false),
                order = table.Column<int>("integer", nullable: false),
                estimate_points = table.Column<int>("integer", nullable: true),
                due_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                created_by = table.Column<Guid>("uuid", nullable: false),
                sprint_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tasks", x => x.id);
                table.ForeignKey(
                    "fk_tasks_projects_project_id",
                    x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_tasks_sprints_sprint_id",
                    x => x.sprint_id,
                    principalSchema: "project",
                    principalTable: "sprints",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_tasks_statuses_status_id",
                    x => x.status_id,
                    principalSchema: "project",
                    principalTable: "statuses",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_tasks_users_created_by",
                    x => x.created_by,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "team_users",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                team_id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                role = table.Column<int>("integer", nullable: false),
                joined_at = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_team_users", x => x.id);
                table.ForeignKey(
                    "fk_team_users_teams_team_id",
                    x => x.team_id,
                    principalSchema: "project",
                    principalTable: "teams",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_team_users_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "Configuration",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                config_type = table.Column<int>("integer", nullable: false),
                value = table.Column<string>("character varying(200)", maxLength: 200, nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_configuration", x => x.id);
                table.ForeignKey(
                    "fk_configuration_companies_company_id",
                    x => x.company_id,
                    principalSchema: "auth",
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_configuration_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "orders",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_number = table.Column<string>("character varying(20)", maxLength: 20, nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                company_id = table.Column<Guid>("uuid", nullable: false),
                total_amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                discount_amount = table.Column<decimal>("numeric", nullable: false),
                total_quantity = table.Column<int>("integer", nullable: false),
                sale_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                status = table.Column<int>("integer", nullable: false),
                payment_method = table.Column<int>("integer", nullable: false),
                notes = table.Column<string>("character varying(1000)", maxLength: 1000, nullable: true),
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
                    principalSchema: "auth",
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_orders_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "users_roles",
            schema: "auth",
            columns: table => new
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
                    principalSchema: "auth",
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_users_roles_roles_role_id",
                    x => x.role_id,
                    principalSchema: "auth",
                    principalTable: "roles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_users_roles_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "orders",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_number = table.Column<string>("character varying(40)", maxLength: 40, nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                customer_id = table.Column<Guid>("uuid", nullable: false),
                total_amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                discount_amount = table.Column<decimal>("numeric", nullable: false),
                total_quantity = table.Column<int>("integer", nullable: false),
                sale_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                status = table.Column<int>("integer", nullable: false),
                payment_method = table.Column<int>("integer", nullable: false),
                notes = table.Column<string>("character varying(1000)", maxLength: 1000, nullable: true),
                employee_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_orders1", x => x.id);
                table.ForeignKey(
                    "fk_orders_customers_customer_id",
                    x => x.customer_id,
                    principalSchema: "basic",
                    principalTable: "customers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_orders_employees_employee_id",
                    x => x.employee_id,
                    principalSchema: "basic",
                    principalTable: "employees",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "products",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                sku = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                barcode = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                description = table.Column<string>("character varying(1000)", maxLength: 1000, nullable: true),
                cost_price = table.Column<decimal>("numeric", nullable: true),
                sales_price = table.Column<decimal>("numeric", nullable: false),
                quantity = table.Column<double>("double precision", nullable: false),
                min_stock_level = table.Column<int>("integer", nullable: true),
                max_stock_level = table.Column<int>("integer", nullable: true),
                image_url = table.Column<string>("character varying(500)", maxLength: 500, nullable: true),
                weight = table.Column<decimal>("numeric", nullable: true),
                dimensions = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                unit_of_measure = table.Column<string>("character varying(20)", maxLength: 20, nullable: true),
                category_id = table.Column<Guid>("uuid", nullable: false),
                supplier_id = table.Column<Guid>("uuid", nullable: true),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
                table.ForeignKey(
                    "fk_products_product_categories_category_id",
                    x => x.category_id,
                    principalSchema: "basic",
                    principalTable: "product_categories",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_products_suppliers_supplier_id",
                    x => x.supplier_id,
                    principalSchema: "basic",
                    principalTable: "suppliers",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "comments",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                feed_id = table.Column<Guid>("uuid", nullable: false),
                parent_comment_id = table.Column<Guid>("uuid", nullable: true),
                text = table.Column<string>("character varying(1024)", maxLength: 1024, nullable: false),
                comment_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_comments1", x => x.id);
                table.ForeignKey(
                    "fk_comments_comments_parent_comment_id",
                    x => x.parent_comment_id,
                    principalSchema: "social_network",
                    principalTable: "comments",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_comments_feeds_feed_id",
                    x => x.feed_id,
                    principalSchema: "social_network",
                    principalTable: "feeds",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_comments_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "shares",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                original_feed_id = table.Column<Guid>("uuid", nullable: false),
                text = table.Column<string>("character varying(512)", maxLength: 512, nullable: true),
                share_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_shares", x => x.id);
                table.ForeignKey(
                    "fk_shares_feeds_original_feed_id",
                    x => x.original_feed_id,
                    principalSchema: "social_network",
                    principalTable: "feeds",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_shares_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "attachments",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                task_id = table.Column<Guid>("uuid", nullable: false),
                file_name = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                file_url = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                file_size = table.Column<long>("bigint", nullable: false),
                uploaded_by = table.Column<Guid>("uuid", nullable: false),
                content_type = table.Column<string>("character varying(50)", maxLength: 50, nullable: true),
                size = table.Column<long>("bigint", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attachments", x => x.id);
                table.ForeignKey(
                    "fk_attachments_tasks_task_id",
                    x => x.task_id,
                    principalSchema: "project",
                    principalTable: "tasks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_attachments_users_uploaded_by",
                    x => x.uploaded_by,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "comments",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                task_id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                content = table.Column<string>("character varying(4096)", maxLength: 4096, nullable: false),
                author_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_comments", x => x.id);
                table.ForeignKey(
                    "fk_comments_tasks_task_id",
                    x => x.task_id,
                    principalSchema: "project",
                    principalTable: "tasks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_comments_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "project_subtasks",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                task_id = table.Column<Guid>("uuid", nullable: false),
                title = table.Column<string>("character varying(256)", maxLength: 256, nullable: false),
                is_completed = table.Column<bool>("boolean", nullable: false),
                order = table.Column<int>("integer", nullable: false),
                completed_at = table.Column<DateTime>("timestamp with time zone", nullable: true),
                due_date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_project_subtasks", x => x.id);
                table.ForeignKey(
                    "fk_project_subtasks_tasks_task_id",
                    x => x.task_id,
                    principalSchema: "project",
                    principalTable: "tasks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "task_assignees",
            schema: "project",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                task_id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                role = table.Column<int>("integer", nullable: false),
                assigned_at = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_task_assignees", x => x.id);
                table.ForeignKey(
                    "fk_task_assignees_tasks_task_id",
                    x => x.task_id,
                    principalSchema: "project",
                    principalTable: "tasks",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_task_assignees_users_user_id",
                    x => x.user_id,
                    principalSchema: "auth",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "order_details",
            schema: "auth",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_id = table.Column<Guid>("uuid", nullable: false),
                module_id = table.Column<Guid>("uuid", nullable: false),
                price = table.Column<decimal>("numeric(18,2)", nullable: false),
                discount_amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                subtotal = table.Column<decimal>("numeric(18,2)", nullable: false),
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
                    principalSchema: "auth",
                    principalTable: "modules",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_order_details_orders_order_id",
                    x => x.order_id,
                    principalSchema: "auth",
                    principalTable: "orders",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "subscriptions",
            schema: "auth",
            columns: table => new
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
                    principalSchema: "auth",
                    principalTable: "companies",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_subscriptions_orders_order_id",
                    x => x.order_id,
                    principalSchema: "auth",
                    principalTable: "orders",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "order_details",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                order_id = table.Column<Guid>("uuid", nullable: false),
                product_id = table.Column<Guid>("uuid", nullable: false),
                price = table.Column<decimal>("numeric(18,2)", nullable: false),
                discount_amount = table.Column<decimal>("numeric(18,2)", nullable: false),
                subtotal = table.Column<decimal>("numeric(18,2)", nullable: false),
                quantity = table.Column<double>("double precision", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_order_details1", x => x.id);
                table.ForeignKey(
                    "fk_order_details_orders_order_id",
                    x => x.order_id,
                    principalSchema: "basic",
                    principalTable: "orders",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_order_details_products_product_id",
                    x => x.product_id,
                    principalSchema: "basic",
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "stock_movements",
            schema: "basic",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                product_id = table.Column<Guid>("uuid", nullable: false),
                quantity = table.Column<double>("double precision", nullable: false),
                date = table.Column<DateTime>("timestamp with time zone", nullable: true),
                price = table.Column<decimal>("numeric", nullable: false),
                type = table.Column<int>("integer", nullable: false),
                reason = table.Column<string>("character varying(255)", maxLength: 255, nullable: true),
                customer_id = table.Column<Guid>("uuid", nullable: true),
                supplier_id = table.Column<Guid>("uuid", nullable: true),
                employee_id = table.Column<Guid>("uuid", nullable: true),
                order_id = table.Column<Guid>("uuid", nullable: true),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stock_movements", x => x.id);
                table.ForeignKey(
                    "fk_stock_movements_customers_customer_id",
                    x => x.customer_id,
                    principalSchema: "basic",
                    principalTable: "customers",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_stock_movements_employees_employee_id",
                    x => x.employee_id,
                    principalSchema: "basic",
                    principalTable: "employees",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_stock_movements_orders_order_id",
                    x => x.order_id,
                    principalSchema: "basic",
                    principalTable: "orders",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_stock_movements_products_product_id",
                    x => x.product_id,
                    principalSchema: "basic",
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_stock_movements_suppliers_supplier_id",
                    x => x.supplier_id,
                    principalSchema: "basic",
                    principalTable: "suppliers",
                    principalColumn: "id");
            });

        migrationBuilder.CreateTable(
            "attachments",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                url = table.Column<string>("character varying(512)", maxLength: 512, nullable: false),
                file_type = table.Column<string>("character varying(64)", maxLength: 64, nullable: false),
                file_size = table.Column<long>("bigint", nullable: false),
                comment_id = table.Column<Guid>("uuid", nullable: false),
                upload_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_attachments1", x => x.id);
                table.ForeignKey(
                    "fk_attachments_comments_comment_id",
                    x => x.comment_id,
                    principalSchema: "social_network",
                    principalTable: "comments",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "likes",
            schema: "social_network",
            columns: table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                profile_id = table.Column<Guid>("uuid", nullable: false),
                feed_id = table.Column<Guid>("uuid", nullable: false),
                comment_id = table.Column<Guid>("uuid", nullable: true),
                like_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true),
                company_id = table.Column<Guid>("uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_likes", x => x.id);
                table.ForeignKey(
                    "fk_likes_comments_comment_id",
                    x => x.comment_id,
                    principalSchema: "social_network",
                    principalTable: "comments",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_likes_feeds_feed_id",
                    x => x.feed_id,
                    principalSchema: "social_network",
                    principalTable: "feeds",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_likes_profiles_profile_id",
                    x => x.profile_id,
                    principalSchema: "auth",
                    principalTable: "profiles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "subscription_credits",
            schema: "auth",
            columns: table => new
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
                    principalSchema: "auth",
                    principalTable: "modules",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_subscription_credits_order_details_order_detail_id",
                    x => x.order_detail_id,
                    principalSchema: "auth",
                    principalTable: "order_details",
                    principalColumn: "id");
                table.ForeignKey(
                    "fk_subscription_credits_subscriptions_subscription_id",
                    x => x.subscription_id,
                    principalSchema: "auth",
                    principalTable: "subscriptions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_addresses_state_id",
            schema: "auth",
            table: "addresses",
            column: "state_id");

        migrationBuilder.CreateIndex(
            "ix_attachments_task_id",
            schema: "project",
            table: "attachments",
            column: "task_id");

        migrationBuilder.CreateIndex(
            "ix_attachments_uploaded_by",
            schema: "project",
            table: "attachments",
            column: "uploaded_by");

        migrationBuilder.CreateIndex(
            "ix_attachments_comment_id",
            schema: "social_network",
            table: "attachments",
            column: "comment_id");

        migrationBuilder.CreateIndex(
            "ix_blocks_blocked_profile_id",
            schema: "social_network",
            table: "blocks",
            column: "blocked_profile_id");

        migrationBuilder.CreateIndex(
            "ix_blocks_profile_id",
            schema: "social_network",
            table: "blocks",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_comments_task_id",
            schema: "project",
            table: "comments",
            column: "task_id");

        migrationBuilder.CreateIndex(
            "ix_comments_user_id",
            schema: "project",
            table: "comments",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_comments_feed_id",
            schema: "social_network",
            table: "comments",
            column: "feed_id");

        migrationBuilder.CreateIndex(
            "ix_comments_parent_comment_id",
            schema: "social_network",
            table: "comments",
            column: "parent_comment_id");

        migrationBuilder.CreateIndex(
            "ix_comments_profile_id",
            schema: "social_network",
            table: "comments",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_companies_address_id",
            schema: "auth",
            table: "companies",
            column: "address_id");

        migrationBuilder.CreateIndex(
            "ix_configuration_company_id",
            schema: "auth",
            table: "Configuration",
            column: "company_id");

        migrationBuilder.CreateIndex(
            "ix_configuration_user_id",
            schema: "auth",
            table: "Configuration",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_customers_person_id",
            schema: "basic",
            table: "customers",
            column: "person_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_employees_person_id",
            schema: "basic",
            table: "employees",
            column: "person_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_employees_position_id",
            schema: "basic",
            table: "employees",
            column: "position_id");

        migrationBuilder.CreateIndex(
            "ix_feeds_original_feed_id",
            schema: "social_network",
            table: "feeds",
            column: "original_feed_id");

        migrationBuilder.CreateIndex(
            "ix_feeds_profile_id",
            schema: "social_network",
            table: "feeds",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_forgotten_passwords_user_id",
            schema: "auth",
            table: "forgotten_passwords",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_friendships_profile_id",
            schema: "social_network",
            table: "friendships",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_friendships_target_profile_id",
            schema: "social_network",
            table: "friendships",
            column: "target_profile_id");

        migrationBuilder.CreateIndex(
            "ix_likes_comment_id",
            schema: "social_network",
            table: "likes",
            column: "comment_id");

        migrationBuilder.CreateIndex(
            "ix_likes_feed_id",
            schema: "social_network",
            table: "likes",
            column: "feed_id");

        migrationBuilder.CreateIndex(
            "ix_likes_profile_id",
            schema: "social_network",
            table: "likes",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_module_id",
            schema: "auth",
            table: "order_details",
            column: "module_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_order_id",
            schema: "auth",
            table: "order_details",
            column: "order_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_order_id1",
            schema: "basic",
            table: "order_details",
            column: "order_id");

        migrationBuilder.CreateIndex(
            "ix_order_details_product_id",
            schema: "basic",
            table: "order_details",
            column: "product_id");

        migrationBuilder.CreateIndex(
            "ix_orders_company_id",
            schema: "auth",
            table: "orders",
            column: "company_id");

        migrationBuilder.CreateIndex(
            "ix_orders_user_id",
            schema: "auth",
            table: "orders",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_orders_customer_id",
            schema: "basic",
            table: "orders",
            column: "customer_id");

        migrationBuilder.CreateIndex(
            "ix_orders_employee_id",
            schema: "basic",
            table: "orders",
            column: "employee_id");

        migrationBuilder.CreateIndex(
            "ix_people_state_id",
            schema: "basic",
            table: "people",
            column: "state_id");

        migrationBuilder.CreateIndex(
            "ix_person_addresses_address_id",
            schema: "basic",
            table: "person_addresses",
            column: "address_id");

        migrationBuilder.CreateIndex(
            "ix_person_addresses_person_id",
            schema: "basic",
            table: "person_addresses",
            column: "person_id");

        migrationBuilder.CreateIndex(
            "ix_products_category_id",
            schema: "basic",
            table: "products",
            column: "category_id");

        migrationBuilder.CreateIndex(
            "ix_products_supplier_id",
            schema: "basic",
            table: "products",
            column: "supplier_id");

        migrationBuilder.CreateIndex(
            "ix_profiles_upload_id",
            schema: "auth",
            table: "profiles",
            column: "upload_id");

        migrationBuilder.CreateIndex(
            "ix_profiles_user_id",
            schema: "auth",
            table: "profiles",
            column: "user_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_project_subtasks_task_id",
            schema: "project",
            table: "project_subtasks",
            column: "task_id");

        migrationBuilder.CreateIndex(
            "ix_reports_reporter_id",
            schema: "social_network",
            table: "reports",
            column: "reporter_id");

        migrationBuilder.CreateIndex(
            "ix_shares_original_feed_id",
            schema: "social_network",
            table: "shares",
            column: "original_feed_id");

        migrationBuilder.CreateIndex(
            "ix_shares_profile_id",
            schema: "social_network",
            table: "shares",
            column: "profile_id");

        migrationBuilder.CreateIndex(
            "ix_sprints_created_by",
            schema: "project",
            table: "sprints",
            column: "created_by");

        migrationBuilder.CreateIndex(
            "ix_sprints_project_id",
            schema: "project",
            table: "sprints",
            column: "project_id");

        migrationBuilder.CreateIndex(
            "ix_statuses_project_id",
            schema: "project",
            table: "statuses",
            column: "project_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_customer_id",
            schema: "basic",
            table: "stock_movements",
            column: "customer_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_employee_id",
            schema: "basic",
            table: "stock_movements",
            column: "employee_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_order_id",
            schema: "basic",
            table: "stock_movements",
            column: "order_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_product_id",
            schema: "basic",
            table: "stock_movements",
            column: "product_id");

        migrationBuilder.CreateIndex(
            "ix_stock_movements_supplier_id",
            schema: "basic",
            table: "stock_movements",
            column: "supplier_id");

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_module_id",
            schema: "auth",
            table: "subscription_credits",
            column: "module_id");

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_order_detail_id",
            schema: "auth",
            table: "subscription_credits",
            column: "order_detail_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_subscription_credits_subscription_id",
            schema: "auth",
            table: "subscription_credits",
            column: "subscription_id");

        migrationBuilder.CreateIndex(
            "ix_subscriptions_company_id",
            schema: "auth",
            table: "subscriptions",
            column: "company_id");

        migrationBuilder.CreateIndex(
            "ix_subscriptions_order_id",
            schema: "auth",
            table: "subscriptions",
            column: "order_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_suppliers_person_id",
            schema: "basic",
            table: "suppliers",
            column: "person_id");

        migrationBuilder.CreateIndex(
            "ix_task_assignees_task_id",
            schema: "project",
            table: "task_assignees",
            column: "task_id");

        migrationBuilder.CreateIndex(
            "ix_task_assignees_user_id",
            schema: "project",
            table: "task_assignees",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_tasks_created_by",
            schema: "project",
            table: "tasks",
            column: "created_by");

        migrationBuilder.CreateIndex(
            "ix_tasks_project_id",
            schema: "project",
            table: "tasks",
            column: "project_id");

        migrationBuilder.CreateIndex(
            "ix_tasks_sprint_id",
            schema: "project",
            table: "tasks",
            column: "sprint_id");

        migrationBuilder.CreateIndex(
            "ix_tasks_status_id",
            schema: "project",
            table: "tasks",
            column: "status_id");

        migrationBuilder.CreateIndex(
            "ix_team_users_team_id",
            schema: "project",
            table: "team_users",
            column: "team_id");

        migrationBuilder.CreateIndex(
            "ix_team_users_user_id",
            schema: "project",
            table: "team_users",
            column: "user_id");

        migrationBuilder.CreateIndex(
            "ix_teams_created_by",
            schema: "project",
            table: "teams",
            column: "created_by");

        migrationBuilder.CreateIndex(
            "ix_teams_project_id",
            schema: "project",
            table: "teams",
            column: "project_id");

        migrationBuilder.CreateIndex(
            "ix_users_roles_company_id",
            schema: "auth",
            table: "users_roles",
            column: "company_id");

        migrationBuilder.CreateIndex(
            "ix_users_roles_role_id",
            schema: "auth",
            table: "users_roles",
            column: "role_id");

        migrationBuilder.CreateIndex(
            "ix_users_roles_user_id",
            schema: "auth",
            table: "users_roles",
            column: "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "attachments",
            "project");

        migrationBuilder.DropTable(
            "attachments",
            "social_network");

        migrationBuilder.DropTable(
            "blocks",
            "social_network");

        migrationBuilder.DropTable(
            "comments",
            "project");

        migrationBuilder.DropTable(
            "Configuration",
            "auth");

        migrationBuilder.DropTable(
            "forgotten_passwords",
            "auth");

        migrationBuilder.DropTable(
            "friendships",
            "social_network");

        migrationBuilder.DropTable(
            "likes",
            "social_network");

        migrationBuilder.DropTable(
            "notifications",
            "auth");

        migrationBuilder.DropTable(
            "order_details",
            "basic");

        migrationBuilder.DropTable(
            "person_addresses",
            "basic");

        migrationBuilder.DropTable(
            "project_subtasks",
            "project");

        migrationBuilder.DropTable(
            "reports",
            "social_network");

        migrationBuilder.DropTable(
            "shares",
            "social_network");

        migrationBuilder.DropTable(
            "stock_movements",
            "basic");

        migrationBuilder.DropTable(
            "subscription_credits",
            "auth");

        migrationBuilder.DropTable(
            "task_assignees",
            "project");

        migrationBuilder.DropTable(
            "team_users",
            "project");

        migrationBuilder.DropTable(
            "users_roles",
            "auth");

        migrationBuilder.DropTable(
            "comments",
            "social_network");

        migrationBuilder.DropTable(
            "orders",
            "basic");

        migrationBuilder.DropTable(
            "products",
            "basic");

        migrationBuilder.DropTable(
            "order_details",
            "auth");

        migrationBuilder.DropTable(
            "subscriptions",
            "auth");

        migrationBuilder.DropTable(
            "tasks",
            "project");

        migrationBuilder.DropTable(
            "teams",
            "project");

        migrationBuilder.DropTable(
            "roles",
            "auth");

        migrationBuilder.DropTable(
            "feeds",
            "social_network");

        migrationBuilder.DropTable(
            "customers",
            "basic");

        migrationBuilder.DropTable(
            "employees",
            "basic");

        migrationBuilder.DropTable(
            "product_categories",
            "basic");

        migrationBuilder.DropTable(
            "suppliers",
            "basic");

        migrationBuilder.DropTable(
            "modules",
            "auth");

        migrationBuilder.DropTable(
            "orders",
            "auth");

        migrationBuilder.DropTable(
            "sprints",
            "project");

        migrationBuilder.DropTable(
            "statuses",
            "project");

        migrationBuilder.DropTable(
            "profiles",
            "auth");

        migrationBuilder.DropTable(
            "positions",
            "basic");

        migrationBuilder.DropTable(
            "people",
            "basic");

        migrationBuilder.DropTable(
            "companies",
            "auth");

        migrationBuilder.DropTable(
            "projects",
            "project");

        migrationBuilder.DropTable(
            "uploads",
            "auth");

        migrationBuilder.DropTable(
            "users",
            "auth");

        migrationBuilder.DropTable(
            "addresses",
            "auth");

        migrationBuilder.DropTable(
            "states",
            "auth");
    }
}
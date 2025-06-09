using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class CompanyAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "address_id",
                table: "companies",
                type: "uuid",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "states",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    uf = table.Column<string>(
                        type: "character varying(2)",
                        maxLength: 2,
                        nullable: false
                    ),
                    created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_states", x => x.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    street = table.Column<string>(
                        type: "character varying(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    number = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    complement = table.Column<string>(
                        type: "character varying(10)",
                        maxLength: 10,
                        nullable: false
                    ),
                    zip_code = table.Column<string>(
                        type: "character varying(9)",
                        maxLength: 9,
                        nullable: false
                    ),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(
                        type: "character varying(30)",
                        maxLength: 30,
                        nullable: false
                    ),
                    created = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    updated = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    deleted = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_addresses", x => x.id);
                    table.ForeignKey(
                        name: "fk_addresses_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "ix_companies_address_id",
                table: "companies",
                column: "address_id"
            );

            migrationBuilder.CreateIndex(
                name: "ix_addresses_state_id",
                table: "addresses",
                column: "state_id"
            );

            migrationBuilder.AddForeignKey(
                name: "fk_companies_addresses_address_id",
                table: "companies",
                column: "address_id",
                principalTable: "addresses",
                principalColumn: "id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_companies_addresses_address_id",
                table: "companies"
            );

            migrationBuilder.DropTable(name: "addresses");

            migrationBuilder.DropTable(name: "states");

            migrationBuilder.DropIndex(name: "ix_companies_address_id", table: "companies");

            migrationBuilder.DropColumn(name: "address_id", table: "companies");
        }
    }
}

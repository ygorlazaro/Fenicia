#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Module.Basic.Migrations;

/// <inheritdoc />
public partial class Person : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "fk_customers_states_state_id",
            "customers");

        migrationBuilder.DropForeignKey(
            "fk_employees_states_state_id",
            "employees");

        migrationBuilder.DropForeignKey(
            "fk_suppliers_states_state_id",
            "suppliers");

        migrationBuilder.DropIndex(
            "ix_employees_state_id",
            "employees");

        migrationBuilder.DropIndex(
            "ix_customers_state_id",
            "customers");

        migrationBuilder.DropColumn(
            "city",
            "suppliers");

        migrationBuilder.DropColumn(
            "complement",
            "suppliers");

        migrationBuilder.DropColumn(
            "name",
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
            "city",
            "employees");

        migrationBuilder.DropColumn(
            "complement",
            "employees");

        migrationBuilder.DropColumn(
            "cpf",
            "employees");

        migrationBuilder.DropColumn(
            "name",
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
            "cpf",
            "customers");

        migrationBuilder.DropColumn(
            "name",
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
            "person_id");

        migrationBuilder.RenameColumn(
            "cpf",
            "suppliers",
            "cnpj");

        migrationBuilder.RenameIndex(
            "ix_suppliers_state_id",
            table: "suppliers",
            newName: "ix_suppliers_person_id");

        migrationBuilder.RenameColumn(
            "selling_price",
            "products",
            "sales_price");

        migrationBuilder.RenameColumn(
            "state_id",
            "employees",
            "person_id");

        migrationBuilder.RenameColumn(
            "state_id",
            "customers",
            "person_id");

        migrationBuilder.AddColumn<Guid>(
            "state_model_id",
            "suppliers",
            "uuid",
            nullable: true);

        migrationBuilder.AlterColumn<decimal>(
            "cost_price",
            "products",
            "numeric",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "numeric");

        migrationBuilder.AddColumn<Guid>(
            "state_model_id",
            "employees",
            "uuid",
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            "state_model_id",
            "customers",
            "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            "people",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                name = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                cpf = table.Column<string>("character varying(14)", maxLength: 14, nullable: true),
                street = table.Column<string>("character varying(100)", maxLength: 100, nullable: false),
                number = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                complement = table.Column<string>("character varying(10)", maxLength: 10, nullable: false),
                neighborhood = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                zip_code = table.Column<string>("character varying(8)", maxLength: 8, nullable: false),
                state_id = table.Column<Guid>("uuid", nullable: false),
                city = table.Column<string>("character varying(50)", maxLength: 50, nullable: false),
                phone_number = table.Column<string>("character varying(20)", maxLength: 20, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_people", x => x.id);
                table.ForeignKey(
                    "fk_people_states_state_id",
                    x => x.state_id,
                    "states",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_suppliers_state_model_id",
            "suppliers",
            "state_model_id");

        migrationBuilder.CreateIndex(
            "ix_employees_person_id",
            "employees",
            "person_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_employees_state_model_id",
            "employees",
            "state_model_id");

        migrationBuilder.CreateIndex(
            "ix_customers_person_id",
            "customers",
            "person_id",
            unique: true);

        migrationBuilder.CreateIndex(
            "ix_customers_state_model_id",
            "customers",
            "state_model_id");

        migrationBuilder.CreateIndex(
            "ix_people_state_id",
            "people",
            "state_id");

        migrationBuilder.AddForeignKey(
            "fk_customers_people_person_id",
            "customers",
            "person_id",
            "people",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_customers_states_state_model_id",
            "customers",
            "state_model_id",
            "states",
            principalColumn: "id");

        migrationBuilder.AddForeignKey(
            "fk_employees_people_person_id",
            "employees",
            "person_id",
            "people",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_employees_states_state_model_id",
            "employees",
            "state_model_id",
            "states",
            principalColumn: "id");

        migrationBuilder.AddForeignKey(
            "fk_suppliers_people_person_id",
            "suppliers",
            "person_id",
            "people",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            "fk_suppliers_states_state_model_id",
            "suppliers",
            "state_model_id",
            "states",
            principalColumn: "id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            "fk_customers_people_person_id",
            "customers");

        migrationBuilder.DropForeignKey(
            "fk_customers_states_state_model_id",
            "customers");

        migrationBuilder.DropForeignKey(
            "fk_employees_people_person_id",
            "employees");

        migrationBuilder.DropForeignKey(
            "fk_employees_states_state_model_id",
            "employees");

        migrationBuilder.DropForeignKey(
            "fk_suppliers_people_person_id",
            "suppliers");

        migrationBuilder.DropForeignKey(
            "fk_suppliers_states_state_model_id",
            "suppliers");

        migrationBuilder.DropTable(
            "people");

        migrationBuilder.DropIndex(
            "ix_suppliers_state_model_id",
            "suppliers");

        migrationBuilder.DropIndex(
            "ix_employees_person_id",
            "employees");

        migrationBuilder.DropIndex(
            "ix_employees_state_model_id",
            "employees");

        migrationBuilder.DropIndex(
            "ix_customers_person_id",
            "customers");

        migrationBuilder.DropIndex(
            "ix_customers_state_model_id",
            "customers");

        migrationBuilder.DropColumn(
            "state_model_id",
            "suppliers");

        migrationBuilder.DropColumn(
            "state_model_id",
            "employees");

        migrationBuilder.DropColumn(
            "state_model_id",
            "customers");

        migrationBuilder.RenameColumn(
            "person_id",
            "suppliers",
            "state_id");

        migrationBuilder.RenameColumn(
            "cnpj",
            "suppliers",
            "cpf");

        migrationBuilder.RenameIndex(
            "ix_suppliers_person_id",
            table: "suppliers",
            newName: "ix_suppliers_state_id");

        migrationBuilder.RenameColumn(
            "sales_price",
            "products",
            "selling_price");

        migrationBuilder.RenameColumn(
            "person_id",
            "employees",
            "state_id");

        migrationBuilder.RenameColumn(
            "person_id",
            "customers",
            "state_id");

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
            "name",
            "suppliers",
            "character varying(50)",
            maxLength: 50,
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

        migrationBuilder.AlterColumn<decimal>(
            "cost_price",
            "products",
            "numeric",
            nullable: false,
            defaultValue: 0m,
            oldClrType: typeof(decimal),
            oldType: "numeric",
            oldNullable: true);

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
            "cpf",
            "employees",
            "character varying(14)",
            maxLength: 14,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            "name",
            "employees",
            "character varying(50)",
            maxLength: 50,
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
            "cpf",
            "customers",
            "character varying(14)",
            maxLength: 14,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            "name",
            "customers",
            "character varying(50)",
            maxLength: 50,
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

        migrationBuilder.CreateIndex(
            "ix_employees_state_id",
            "employees",
            "state_id");

        migrationBuilder.CreateIndex(
            "ix_customers_state_id",
            "customers",
            "state_id");

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
            "fk_suppliers_states_state_id",
            "suppliers",
            "state_id",
            "states",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }
}
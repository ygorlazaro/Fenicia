using System;

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Module.Basic.Migrations
{
    /// <inheritdoc />
    public partial class Person : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_states_state_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_states_state_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_states_state_id",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_employees_state_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_customers_state_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "city",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "name",
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
                name: "city",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "complement",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "cpf",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "name",
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
                name: "cpf",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "name",
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
                newName: "person_id");

            migrationBuilder.RenameColumn(
                name: "cpf",
                table: "suppliers",
                newName: "cnpj");

            migrationBuilder.RenameIndex(
                name: "ix_suppliers_state_id",
                table: "suppliers",
                newName: "ix_suppliers_person_id");

            migrationBuilder.RenameColumn(
                name: "selling_price",
                table: "products",
                newName: "sales_price");

            migrationBuilder.RenameColumn(
                name: "state_id",
                table: "employees",
                newName: "person_id");

            migrationBuilder.RenameColumn(
                name: "state_id",
                table: "customers",
                newName: "person_id");

            migrationBuilder.AddColumn<Guid>(
                name: "state_model_id",
                table: "suppliers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "cost_price",
                table: "products",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "state_model_id",
                table: "employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "state_model_id",
                table: "customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "people",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    street = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    complement = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    neighborhood = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    zip_code = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    state_id = table.Column<Guid>(type: "uuid", nullable: false),
                    city = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_people", x => x.id);
                    table.ForeignKey(
                        name: "fk_people_states_state_id",
                        column: x => x.state_id,
                        principalTable: "states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_state_model_id",
                table: "suppliers",
                column: "state_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_employees_person_id",
                table: "employees",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_employees_state_model_id",
                table: "employees",
                column: "state_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_person_id",
                table: "customers",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_customers_state_model_id",
                table: "customers",
                column: "state_model_id");

            migrationBuilder.CreateIndex(
                name: "ix_people_state_id",
                table: "people",
                column: "state_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customers_people_person_id",
                table: "customers",
                column: "person_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_customers_states_state_model_id",
                table: "customers",
                column: "state_model_id",
                principalTable: "states",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_employees_people_person_id",
                table: "employees",
                column: "person_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_employees_states_state_model_id",
                table: "employees",
                column: "state_model_id",
                principalTable: "states",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_people_person_id",
                table: "suppliers",
                column: "person_id",
                principalTable: "people",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_suppliers_states_state_model_id",
                table: "suppliers",
                column: "state_model_id",
                principalTable: "states",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customers_people_person_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_customers_states_state_model_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_people_person_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_employees_states_state_model_id",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_people_person_id",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "fk_suppliers_states_state_model_id",
                table: "suppliers");

            migrationBuilder.DropTable(
                name: "people");

            migrationBuilder.DropIndex(
                name: "ix_suppliers_state_model_id",
                table: "suppliers");

            migrationBuilder.DropIndex(
                name: "ix_employees_person_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_employees_state_model_id",
                table: "employees");

            migrationBuilder.DropIndex(
                name: "ix_customers_person_id",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "ix_customers_state_model_id",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "state_model_id",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "state_model_id",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "state_model_id",
                table: "customers");

            migrationBuilder.RenameColumn(
                name: "person_id",
                table: "suppliers",
                newName: "state_id");

            migrationBuilder.RenameColumn(
                name: "cnpj",
                table: "suppliers",
                newName: "cpf");

            migrationBuilder.RenameIndex(
                name: "ix_suppliers_person_id",
                table: "suppliers",
                newName: "ix_suppliers_state_id");

            migrationBuilder.RenameColumn(
                name: "sales_price",
                table: "products",
                newName: "selling_price");

            migrationBuilder.RenameColumn(
                name: "person_id",
                table: "employees",
                newName: "state_id");

            migrationBuilder.RenameColumn(
                name: "person_id",
                table: "customers",
                newName: "state_id");

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
                name: "name",
                table: "suppliers",
                type: "character varying(50)",
                maxLength: 50,
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

            migrationBuilder.AlterColumn<decimal>(
                name: "cost_price",
                table: "products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

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
                name: "cpf",
                table: "employees",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "employees",
                type: "character varying(50)",
                maxLength: 50,
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
                name: "cpf",
                table: "customers",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "customers",
                type: "character varying(50)",
                maxLength: 50,
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

            migrationBuilder.CreateIndex(
                name: "ix_employees_state_id",
                table: "employees",
                column: "state_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_state_id",
                table: "customers",
                column: "state_id");

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
                name: "fk_suppliers_states_state_id",
                table: "suppliers",
                column: "state_id",
                principalTable: "states",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
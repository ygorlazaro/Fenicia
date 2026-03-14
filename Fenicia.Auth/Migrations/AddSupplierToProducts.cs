#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class AddSupplierToProducts : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("fk_suppliers_people_person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropIndex("ix_suppliers_person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropColumn("person_model_id", schema: "basic", table: "suppliers");

        migrationBuilder.AddColumn<Guid>("supplier_id", schema: "basic", table: "products", type: "uuid", nullable: true);

        migrationBuilder.CreateIndex("ix_suppliers_person_id", schema: "basic", table: "suppliers", column: "person_id");

        migrationBuilder.CreateIndex("ix_products_supplier_id", schema: "basic", table: "products", column: "supplier_id");

        migrationBuilder.AddForeignKey("fk_products_suppliers_supplier_id", schema: "basic", table: "products", column: "supplier_id", principalSchema: "basic", principalTable: "suppliers", principalColumn: "id");

        migrationBuilder.AddForeignKey("fk_suppliers_people_person_id", schema: "basic", table: "suppliers", column: "person_id", principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey("fk_products_suppliers_supplier_id", schema: "basic", table: "products");

        migrationBuilder.DropForeignKey("fk_suppliers_people_person_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropIndex("ix_suppliers_person_id", schema: "basic", table: "suppliers");

        migrationBuilder.DropIndex("ix_products_supplier_id", schema: "basic", table: "products");

        migrationBuilder.DropColumn("supplier_id", schema: "basic", table: "products");

        migrationBuilder.AddColumn<Guid>("person_model_id", schema: "basic", table: "suppliers", type: "uuid", nullable: false, defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateIndex("ix_suppliers_person_model_id", schema: "basic", table: "suppliers", column: "person_model_id");

        migrationBuilder.AddForeignKey("fk_suppliers_people_person_model_id", schema: "basic", table: "suppliers", column: "person_model_id", principalSchema: "basic", principalTable: "people", principalColumn: "id", onDelete: ReferentialAction.Cascade);
    }
}
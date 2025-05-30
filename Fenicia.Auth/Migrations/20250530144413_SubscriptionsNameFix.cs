using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations
{
    /// <inheritdoc />
    public partial class SubscriptionsNameFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subscrption_credits_modules_module_id",
                table: "subscrption_credits");

            migrationBuilder.DropForeignKey(
                name: "fk_subscrption_credits_subscriptions_subscription_id",
                table: "subscrption_credits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscrption_credits",
                table: "subscrption_credits");

            migrationBuilder.RenameTable(
                name: "subscrption_credits",
                newName: "subscription_credits");

            migrationBuilder.RenameIndex(
                name: "ix_subscrption_credits_subscription_id",
                table: "subscription_credits",
                newName: "ix_subscription_credits_subscription_id");

            migrationBuilder.RenameIndex(
                name: "ix_subscrption_credits_module_id",
                table: "subscription_credits",
                newName: "ix_subscription_credits_module_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscription_credits",
                table: "subscription_credits",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_credits_modules_module_id",
                table: "subscription_credits",
                column: "module_id",
                principalTable: "modules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscription_credits_subscriptions_subscription_id",
                table: "subscription_credits",
                column: "subscription_id",
                principalTable: "subscriptions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_subscription_credits_modules_module_id",
                table: "subscription_credits");

            migrationBuilder.DropForeignKey(
                name: "fk_subscription_credits_subscriptions_subscription_id",
                table: "subscription_credits");

            migrationBuilder.DropPrimaryKey(
                name: "pk_subscription_credits",
                table: "subscription_credits");

            migrationBuilder.RenameTable(
                name: "subscription_credits",
                newName: "subscrption_credits");

            migrationBuilder.RenameIndex(
                name: "ix_subscription_credits_subscription_id",
                table: "subscrption_credits",
                newName: "ix_subscrption_credits_subscription_id");

            migrationBuilder.RenameIndex(
                name: "ix_subscription_credits_module_id",
                table: "subscrption_credits",
                newName: "ix_subscrption_credits_module_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_subscrption_credits",
                table: "subscrption_credits",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_subscrption_credits_modules_module_id",
                table: "subscrption_credits",
                column: "module_id",
                principalTable: "modules",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_subscrption_credits_subscriptions_subscription_id",
                table: "subscrption_credits",
                column: "subscription_id",
                principalTable: "subscriptions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace Fenicia.Module.SocialNetwork.Migrations;

/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            "users",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                username = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                name = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                email = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                image_url = table.Column<string>("character varying(48)", maxLength: 48, nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table => { table.PrimaryKey("pk_users", x => x.id); });

        migrationBuilder.CreateTable(
            "feeds",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                text = table.Column<string>("character varying(512)", maxLength: 512, nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_feeds", x => x.id);
                table.ForeignKey(
                    "fk_feeds_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            "followers",
            table => new
            {
                id = table.Column<Guid>("uuid", nullable: false),
                user_id = table.Column<Guid>("uuid", nullable: false),
                follower_id = table.Column<Guid>("uuid", nullable: false),
                follow_date = table.Column<DateTime>("timestamp with time zone", nullable: false),
                is_active = table.Column<bool>("boolean", nullable: false),
                created = table.Column<DateTime>("timestamp with time zone", nullable: false),
                updated = table.Column<DateTime>("timestamp with time zone", nullable: true),
                deleted = table.Column<DateTime>("timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_followers", x => x.id);
                table.ForeignKey(
                    "fk_followers_users_follower_id",
                    x => x.follower_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    "fk_followers_users_user_id",
                    x => x.user_id,
                    "users",
                    "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            "ix_feeds_user_id",
            "feeds",
            "user_id");

        migrationBuilder.CreateIndex(
            "ix_followers_follower_id",
            "followers",
            "follower_id");

        migrationBuilder.CreateIndex(
            "ix_followers_user_id",
            "followers",
            "user_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            "feeds");

        migrationBuilder.DropTable(
            "followers");

        migrationBuilder.DropTable(
            "users");
    }
}
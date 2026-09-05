using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fenicia.Auth.Migrations;

/// <inheritdoc />
public partial class FixSocialNetworkCommentsForeignKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE social_network.comments DROP CONSTRAINT IF EXISTS fk_comments_users_user_id;
            ALTER TABLE social_network.comments DROP CONSTRAINT IF EXISTS fk_comments_profiles_profile_id;
            ALTER TABLE social_network.comments
                ADD CONSTRAINT fk_comments_profiles_profile_id
                FOREIGN KEY (profile_id)
                REFERENCES social_network.profiles (id)
                ON DELETE CASCADE;
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            ALTER TABLE social_network.comments DROP CONSTRAINT IF EXISTS fk_comments_profiles_profile_id;
            ALTER TABLE social_network.comments
                ADD CONSTRAINT fk_comments_users_user_id
                FOREIGN KEY (user_id)
                REFERENCES auth.users (id)
                ON DELETE CASCADE;
        ");
    }
}

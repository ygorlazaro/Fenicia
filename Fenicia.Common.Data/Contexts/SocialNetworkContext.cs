using Fenicia.Common.Data.Models.SocialNetwork;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : DbContext(options)
{
    public DbSet<UserModel> Users
    {
        get;
        set;
    }

    public DbSet<FollowerModel> Followers
    {
        get;
        set;
    }

    public DbSet<FeedModel> Feeds
    {
        get;
        set;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

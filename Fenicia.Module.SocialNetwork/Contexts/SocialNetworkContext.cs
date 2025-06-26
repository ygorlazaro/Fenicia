namespace Fenicia.Module.SocialNetwork.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

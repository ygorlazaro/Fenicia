using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Database.Contexts;

public class SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

using Fenicia.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Contexts;

public class SocialNetworkContext(DbContextOptions<SocialNetworkContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

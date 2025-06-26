namespace Fenicia.Module.Plus.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class PlusContext(DbContextOptions<PlusContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

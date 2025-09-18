namespace Fenicia.Common.Database.Contexts;

using Database;

using Microsoft.EntityFrameworkCore;

public class PosContext : DbContext
{
    public PosContext(DbContextOptions<PosContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

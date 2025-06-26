namespace Fenicia.Module.POS.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class PosContext(DbContextOptions<PosContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

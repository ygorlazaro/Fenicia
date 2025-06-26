namespace Fenicia.Module.HR.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class HrContext(DbContextOptions<HrContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

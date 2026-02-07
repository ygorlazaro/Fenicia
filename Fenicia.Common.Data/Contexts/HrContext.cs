using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class HrContext(DbContextOptions<HrContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
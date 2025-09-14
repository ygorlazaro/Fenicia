namespace Fenicia.Common.Database.Contexts;

using Database;

using Microsoft.EntityFrameworkCore;

public class PerformanceEvaluationContext : DbContext
{
    public PerformanceEvaluationContext(DbContextOptions<PerformanceEvaluationContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

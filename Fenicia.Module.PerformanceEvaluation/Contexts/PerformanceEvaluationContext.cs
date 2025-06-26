namespace Fenicia.Module.PerformanceEvaluation.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class PerformanceEvaluationContext(DbContextOptions<PerformanceEvaluationContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

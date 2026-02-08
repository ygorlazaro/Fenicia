using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class PerformanceEvaluationContext(DbContextOptions<PerformanceEvaluationContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
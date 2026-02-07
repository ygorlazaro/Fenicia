using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class ProjectContext(DbContextOptions<ProjectContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
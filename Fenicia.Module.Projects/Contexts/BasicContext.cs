namespace Fenicia.Module.Projects.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class ProjectContext(DbContextOptions<ProjectContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

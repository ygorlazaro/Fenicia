using Fenicia.Common.Api.Providers;
using Fenicia.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Contexts;

public class ProjectContext(DbContextOptions<ProjectContext> options, TenantProvider tenantProvider, IConfiguration configuration) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
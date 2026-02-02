using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Database.Contexts;

public class ContractsContext(DbContextOptions<ContractsContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

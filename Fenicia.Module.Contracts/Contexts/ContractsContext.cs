namespace Fenicia.Module.Contracts.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class ContractsContext(DbContextOptions<ContractsContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

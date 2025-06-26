namespace Fenicia.Module.Accounting.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class AccountingContext(DbContextOptions<AccountingContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

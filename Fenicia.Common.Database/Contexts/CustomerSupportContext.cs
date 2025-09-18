namespace Fenicia.Common.Database.Contexts;

using Database;

using Microsoft.EntityFrameworkCore;

public class CustomerSupportContext : DbContext
{
    public CustomerSupportContext(DbContextOptions<CustomerSupportContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

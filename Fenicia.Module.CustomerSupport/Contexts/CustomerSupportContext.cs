using Fenicia.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.CustomerSupport.Contexts;

public class CustomerSupportContext(DbContextOptions<CustomerSupportContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
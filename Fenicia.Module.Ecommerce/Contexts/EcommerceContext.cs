namespace Fenicia.Module.Ecommerce.Contexts;

using Common.Database;

using Microsoft.EntityFrameworkCore;

public class EcommerceContext(DbContextOptions<EcommerceContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

using Fenicia.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Plus.Contexts;

public class PlusContext(DbContextOptions<PlusContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
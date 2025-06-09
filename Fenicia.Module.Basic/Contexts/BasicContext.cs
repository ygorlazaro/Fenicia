using Fenicia.Common.Api.Providers;
using Fenicia.Common.Database;
using Fenicia.Module.Basic.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Contexts;

public class BasicContext(
    DbContextOptions<BasicContext> options,
    TenantProvider tenantProvider,
    IConfiguration configuration
) : DbContext(options)
{
    public DbSet<StateModel> States { get; set; } = null!;

    public DbSet<AddressModel> Addresses { get; set; } = null!;

    public DbSet<CustomerModel> Customers { get; set; } = null!;

    public DbSet<EmployeeModel> Employees { get; set; } = null!;

    public DbSet<PositionModel> Positions { get; set; } = null!;

    public DbSet<ProductCategoryModel> ProductCategories { get; set; } = null!;

    public DbSet<ProductModel> Products { get; set; } = null!;

    public DbSet<StockMovementModel> StockMovements { get; set; } = null!;

    public DbSet<SupplierModel> Suppliers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

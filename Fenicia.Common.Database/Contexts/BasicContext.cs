using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Database.Contexts;

public class BasicContext(DbContextOptions<BasicContext> options) : DbContext(options)
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

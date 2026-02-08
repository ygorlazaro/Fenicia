using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class BasicContext(DbContextOptions<BasicContext> options) : DbContext(options)
{
    public DbSet<StateModel> States { get; set; }

    public DbSet<CustomerModel> Customers { get; set; }

    public DbSet<EmployeeModel> Employees { get; set; }

    public DbSet<PositionModel> Positions { get; set; }

    public DbSet<ProductCategoryModel> ProductCategories { get; set; }

    public DbSet<ProductModel> Products { get; set; }

    public DbSet<StockMovementModel> StockMovements { get; set; }

    public DbSet<SupplierModel> Suppliers { get; set; }

    public DbSet<OrderModel> Orders { get; set; }

    public DbSet<OrderDetailModel> OrderDetails { get; set; }

    public DbSet<PersonModel> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}
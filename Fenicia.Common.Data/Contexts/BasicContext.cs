using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public class BasicContext(DbContextOptions<BasicContext> options) : DbContext(options)
{
    public DbSet<StateModel> States
    {
        get;
        set;
    }

        = null!;

    public DbSet<CustomerModel> Customers
    {
        get;
        set;
    }

        = null!;

    public DbSet<EmployeeModel> Employees
    {
        get;
        set;
    }

        = null!;

    public DbSet<PositionModel> Positions
    {
        get;
        set;
    }

        = null!;

    public DbSet<ProductCategoryModel> ProductCategories
    {
        get;
        set;
    }

        = null!;

    public DbSet<ProductModel> Products
    {
        get;
        set;
    }

        = null!;

    public DbSet<StockMovementModel> StockMovements
    {
        get;
        set;
    }

        = null!;

    public DbSet<SupplierModel> Suppliers
    {
        get;
        set;
    }

        = null!;

    public DbSet<OrderModel> Orders
    {
        get;
        set;
    }

        = null!;

    public DbSet<OrderDetailModel> OrderDetails
    {
        get;
        set;
    }

        = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        PostgresDateTimeOffsetSupport.Init(modelBuilder);
        SoftDeleteQueryExtension.AddSoftDeleteSupport(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}

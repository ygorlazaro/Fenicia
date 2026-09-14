using Fenicia.Common.Data.Models.Basic;
using Microsoft.EntityFrameworkCore;
using OrderDetailModel = Fenicia.Common.Data.Models.Basic.OrderDetailModel;
using OrderModel = Fenicia.Common.Data.Models.Basic.OrderModel;

namespace Fenicia.Common.Data.Contexts;

#pragma warning disable SA1601 // Partial elements should be documented
public partial class DefaultContext
#pragma warning restore SA1601 // Partial elements should be documented
{
    public DbSet<CustomerModel> BasicCustomers { get; set; }

    public DbSet<EmployeeModel> BasicEmployees { get; set; }

    public DbSet<PositionModel> BasicPositions { get; set; }

    public DbSet<ProductCategoryModel> BasicProductCategories { get; set; }

    public DbSet<ProductModel> BasicProducts { get; set; }

    public DbSet<StockMovementModel> BasicStockMovements { get; set; }

    public DbSet<SupplierModel> BasicSuppliers { get; set; }

    public DbSet<OrderModel> BasicOrders { get; set; }

    public DbSet<OrderDetailModel> BasicOrderDetails { get; set; }

    public DbSet<PersonModel> BasicPeople { get; set; }

    public DbSet<PersonAddressModel> BasicPersonAddresses { get; set; }
}
using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Auth.Domains.Address;
using Fenicia.Auth.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.Interfaces;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Auth.Domains.Person;
using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Auth.Domains.PersonAddress;
using Fenicia.Auth.Domains.PersonAddress.Interfaces;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Auth.Domains.State;
using Fenicia.Auth.Domains.State.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;

namespace Fenicia.Module.Basic;

internal static class Program
{
    public static void Main(string[] args)
    {
        FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors()
            .AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
                builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
                builder.Services.AddScoped<IPositionRepository, PositionRepository>();
                builder.Services.AddScoped<IProductRepository, ProductRepository>();
                builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
                builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
                builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
                builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
                builder.Services.AddScoped<IStockMovementService, StockMovementService>();
                builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
                builder.Services.AddScoped<IPositionService, PositionService>();
                builder.Services.AddScoped<IProductService, ProductService>();
                builder.Services.AddScoped<ISupplierService, SupplierService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<ICustomerService, CustomerService>();
                builder.Services.AddScoped<IEmployeeService, EmployeeService>();
                builder.Services.AddScoped<IInventoryService, InventoryService>();
                builder.Services.AddScoped<IDataSourceService, DataSourceService>();
                builder.Services.AddScoped<IDashboardService, DashboardService>();
            }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") == "true")
        {
            return;
        }

        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
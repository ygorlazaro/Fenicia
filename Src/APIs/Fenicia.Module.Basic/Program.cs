using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.Supplier;
using ProductRepository = Fenicia.Module.Basic.Domains.Product.ProductRepository;
using StockMovementRepository = Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository;

namespace Fenicia.Module.Basic;

public class Program
{
    public static void Main(string[] args)
    {
        var tenantId = FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
    {
        builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<CustomerRepository>();
        builder.Services.AddScoped<PersonRepository>();
        builder.Services.AddScoped<AddressRepository>();
        builder.Services.AddScoped<PersonAddressRepository>();
        builder.Services.AddScoped<EmployeeRepository>();
        builder.Services.AddScoped<PositionRepository>();
        builder.Services.AddScoped<ProductRepository>();
        builder.Services.AddScoped<ProductCategoryRepository>();
        builder.Services.AddScoped<StockMovementRepository>();
        builder.Services.AddScoped<OrderDetailRepository>();
        builder.Services.AddScoped<SupplierRepository>();
        builder.Services.AddScoped<StateRepository>();
    }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") != "true")
        {
            app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.Run();
        }
    }
}

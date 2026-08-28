using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Supplier;

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
            builder.Services.AddScoped<DashboardRepository>();
            builder.Services.AddScoped<EmployeeRepository>();
            builder.Services.AddScoped<PositionRepository>();
            builder.Services.AddScoped<ProductRepository>();
            builder.Services.AddScoped<StockMovementRepository>();
            builder.Services.AddScoped<OrderDetailRepository>();
            builder.Services.AddScoped<SupplierRepository>();
        }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") != "true")
        {
            app.Run();
        }
    }
}

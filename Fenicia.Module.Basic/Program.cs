using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Basic;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var tenantId = FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging()
            .AddFeniciaRateLimiting(configuration)
            .AddFeniciaCors()
            .AddFeniciaAuthentication(configuration)
            .AddFeniciaControllers()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();
            })
            .AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth", tenantId);

        builder.Start("/basic", "basic");
    }
}

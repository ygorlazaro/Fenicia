using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Basic;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var tenantId = FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging()
            .AddFeniciaDbContext<BasicContext>(configuration, "Fenicia.Module.Basic", "Basic", tenantId)
            .AddFeniciaRateLimiting(configuration)
            .AddFeniciaCors()
            .AddFeniciaAuthentication(configuration)
            .AddFeniciaControllers()
            .AddFeniciaDependencyInjection(() =>
            {
              
            });

        builder.Start("/basic", "basic");
    }
}

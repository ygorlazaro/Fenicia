using Microsoft.Extensions.DependencyInjection;
using Fenicia.Common.API.Startup;

namespace Fenicia.Module.Basic.Tests.Integration;

public class BasicProgramIntegrationTests
{
    [Fact]
    public void Program_StartupBuildsSuccessfully()
    {
        var args = new[] { "--tenant=test-tenant" };
        var tenantId = Fenicia.Common.API.FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
        {
            builder.Services.AddSingleton<Common.Data.ICompanyContext, Common.Data.CompanyContext>();
            builder.Services.AddHttpContextAccessor();
        }).AddFeniciaDbContext<Common.Data.Contexts.DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        Assert.NotNull(app);
    }
}

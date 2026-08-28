
        
        {
    {
    }
{
}
        }).AddFeniciaDbContext<Common.Data.Contexts.DefaultContext>(configuration, "Fenicia.Auth", "Auth");
        app.UseFeniciaLocalization();
        Assert.NotNull(app);
        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<Common.Data.ICompanyContext, Common.Data.CompanyContext>();
    [Fact]
namespace Fenicia.Module.Basic.Tests.Integration;
public class BasicProgramIntegrationTests
    public void Program_StartupBuildsSuccessfully()
using Fenicia.Common.API.Startup;
using Microsoft.Extensions.DependencyInjection;
        var app = builder.Build();
        var args = new[] { "--tenant=test-tenant" };
        var tenantId = Fenicia.Common.API.FeniciaModuleLoader.Load(args, out var configuration, out var builder);

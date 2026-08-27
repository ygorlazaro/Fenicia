using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using MediatR;
using Xunit;

namespace Fenicia.Module.Basic.Tests.Integration;

public class BasicProgramIntegrationTests
{
    [Fact]
    public void Program_StartupBuildsSuccessfully()
    {
        var args = new[] { "--tenant=test-tenant" };
        var tenantId = Fenicia.Common.API.FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Fenicia.Module.Basic.Program).Assembly));

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization().AddFeniciaDependencyInjection(() =>
        {
            builder.Services.AddSingleton<Fenicia.Common.Data.ICompanyContext, Fenicia.Common.Data.CompanyContext>();
            builder.Services.AddHttpContextAccessor();
        }).AddFeniciaDbContext<Fenicia.Common.Data.Contexts.DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        Assert.NotNull(app);
    }
}

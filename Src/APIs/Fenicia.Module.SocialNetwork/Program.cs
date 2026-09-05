using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.SocialNetwork.Domains.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;

namespace Fenicia.Module.SocialNetwork;

public class Program
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
                builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
                builder.Services.AddScoped<IProfileService, ProfileService>();
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
        app.Run();
    }
}
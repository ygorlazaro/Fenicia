using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.LoginAttempt.Interfaces;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Module.Interfaces;
using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Auth.Domains.Register;
using Fenicia.Auth.Domains.Register.Interfaces;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.Interfaces;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.Interfaces;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Repositories;
using Fenicia.Externals.Email;

namespace Fenicia.Auth;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.Common.json");

        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configuration.AddJsonFile(commonApiSettingsPath, false, true);
        configuration.AddEnvironmentVariables();

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors()
            .AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaLocalization()
            .AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
                builder.Services.AddScoped<IRoleRepository, RoleRepository>();
                builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
                builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
                builder.Services.AddScoped<IForgotPasswordRepository, ForgotPasswordRepository>();
                builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
                builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
                builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
                builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                builder.Services.AddScoped<ISecurityService, SecurityService>();
                builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();
                builder.Services.AddScoped<ICompanyService, CompanyService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IUserRoleService, UserRoleService>();
                builder.Services.AddScoped<IRoleService, RoleService>();
                builder.Services.AddScoped<IModuleService, ModuleService>();
                builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
                builder.Services.AddScoped<ITokenService, TokenService>();
                builder.Services.AddScoped<IRegisterService, RegisterService>();
                builder.Services.AddScoped<INotificationService, NotificationService>();
                builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
                builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
                builder.Services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
                builder.Services.AddScoped<IOrderService, OrderService>();
            }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        var app = builder.Build();
        app.UseFeniciaLocalization();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_TESTING") != "true")
        {
            DbInitializer.InitializeAsync(app.Services).GetAwaiter().GetResult();
        }

        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
using System.Text;
using System.Text.Json.Serialization;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common;
using Fenicia.Common.Api.Middlewares;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

using Serilog;

namespace Fenicia.Auth;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog(
            (context, loggerConfiguration) =>
            {
                loggerConfiguration.WriteTo.Console();
                loggerConfiguration.ReadFrom.Configuration(context.Configuration);
            }
        );

        var configuration = builder.Configuration;

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        var key = Encoding.ASCII.GetBytes(
            configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret)
        );

        var connectionString = configuration.GetConnectionString("AuthConnection");

        builder.Services.AddTransient<ICompanyService, CompanyService>();
        builder.Services.AddTransient<IModuleService, ModuleService>();
        builder.Services.AddTransient<IOrderService, OrderService>();
        builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddTransient<ISecurityService, SecurityService>();
        builder.Services.AddTransient<ISubscriptionCreditService, SubscriptionCreditService>();
        builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddTransient<IUserRoleService, UserRoleService>();
        builder.Services.AddTransient<IUserService, UserService>();

        builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
        builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
        builder.Services.AddTransient<IOrderRepository, OrderRepository>();
        builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddTransient<IRoleRepository, RoleRepository>();
        builder.Services.AddTransient<
            ISubscriptionCreditRepository,
            SubscriptionCreditRepository
        >();
        builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddDbContext<AuthContext>(x =>
        {
            x.UseNpgsql(connectionString)
                .EnableSensitiveDataLogging()
                .UseSnakeCaseNamingConvention();
        });

        builder
            .Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.ClaimsIssuer = "AuthService";
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder
            .Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.AllowTrailingCommas = false;
                x.JsonSerializerOptions.MaxDepth = 0;
                x.JsonSerializerOptions.DefaultIgnoreCondition =
                    JsonIgnoreCondition.WhenWritingNull;
            });
        builder.Services.AddOpenApi();

        var app = builder.Build();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(x =>
            {
                x.WithDarkModeToggle(true).WithTheme(ScalarTheme.Purple).WithClientButton(true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

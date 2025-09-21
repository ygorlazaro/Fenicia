namespace Fenicia.Auth;

using System.Text;
using System.Text.Json.Serialization;

using AspNetCoreRateLimit;

using Common;
using Common.API.Middlewares;
using Common.Database.Contexts;
using Common.Externals.Email;

using Domains.DataCache;

using Domains.Company;
using Domains.ForgotPassword;
using Domains.LoginAttempt;
using Fenicia.Auth.Domains.Module;
using Domains.Order;
using Domains.RefreshToken;
using Domains.Role;
using Domains.Security;
using Domains.State;
using Domains.Subscription;
using Domains.SubscriptionCredit;
using Domains.Token;
using Domains.User;
using Domains.UserRole;
using Fenicia.Common.Migrations.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

using Serilog;

using StackExchange.Redis;

public static class Program
{
    public static void Main(string[] args)
    {
        var configBuilder = new ConfigurationManager();
        var commonApiSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");
        if (!File.Exists(commonApiSettingsPath))
        {
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");
        }

        configBuilder.AddJsonFile(commonApiSettingsPath, optional: false, reloadOnChange: true);
        var configuration = configBuilder;

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configuration);

        Program.BuildLogging(builder);
        Program.BuildRateLimiting(builder, configuration);
        Program.BuildDependencyInjection(builder);
        Program.BuildDatabaseConnection(configuration, builder);
        Program.BuildCors(builder);
        Program.BuildControllers(configuration, builder);

        Program.StartApplication(builder);
    }

    private static void StartApplication(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseResponseCompression();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(x =>
            {
                x.WithDarkModeToggle(showDarkModeToggle: true).WithTheme(ScalarTheme.Purple).WithClientButton(showButton: true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseHttpsRedirection();
        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");

        app.UseHsts();
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseXfo(options => options.Deny());

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIpRateLimiting();
        app.MapControllers();

        app.Run();
    }

    private static void BuildControllers(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecretMessage));
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.ClaimsIssuer = "AuthService";
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Um ou mais erros de validação ocorreram.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = context.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        builder.Services.AddControllers().AddJsonOptions(x =>
        {
            x.JsonSerializerOptions.AllowTrailingCommas = false;
            x.JsonSerializerOptions.MaxDepth = 0;
            x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddOpenApi();
    }

    private static void BuildCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("RestrictedCors", policy => { policy.WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });

            options.AddPolicy("DevCors", policy => { policy.WithOrigins("http://localhost:5144", "http://localhost:3000", "http://127.0.0.1:5144").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
        });
    }

    private static void BuildDatabaseConnection(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var connectionString = configuration.GetConnectionString("Auth");

        builder.Services.AddDbContextPool<AuthContext>(x => x.UseNpgsql(connectionString, b => b.MigrationsAssembly("Fenicia.Auth"))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention());
    }

    private static void BuildDependencyInjection(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse("localhost", ignoreUnknown: true);
            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(config);
        });

        builder.Services.AddTransient<ICompanyService, CompanyService>();
        builder.Services.AddTransient<IDataCacheService, RedisDataCacheService>();
        builder.Services.AddTransient<IForgotPasswordService, ForgotPasswordService>();
        builder.Services.AddTransient<ILoginAttemptService, RedisLoginAttemptService>();
        builder.Services.AddTransient<IModuleService, ModuleService>();
        builder.Services.AddTransient<IOrderService, OrderService>();
        builder.Services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddTransient<ISecurityService, SecurityService>();
        builder.Services.AddTransient<ISubscriptionCreditService, SubscriptionCreditService>();
        builder.Services.AddTransient<ISubscriptionService, SubscriptionService>();
        builder.Services.AddTransient<IStateService, StateService>();
        builder.Services.AddTransient<ITokenService, TokenService>();
        builder.Services.AddTransient<IUserRoleService, UserRoleService>();
        builder.Services.AddTransient<IUserService, UserService>();
        builder.Services.AddTransient<IMigrationService, MigrationService>();

        builder.Services.AddTransient<ICompanyRepository, CompanyRepository>();
        builder.Services.AddTransient<IForgotPasswordRepository, ForgotPasswordRepository>();
        builder.Services.AddTransient<IModuleRepository, ModuleRepository>();
        builder.Services.AddTransient<IOrderRepository, OrderRepository>();
        builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddTransient<IRoleRepository, RoleRepository>();
        builder.Services.AddTransient<ISubscriptionCreditRepository, SubscriptionCreditRepository>();
        builder.Services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
        builder.Services.AddTransient<IStateRepository, StateRepository>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

        builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();

        builder.Services.AddResponseCompression(config => { config.EnableForHttps = true; });
    }

    private static void BuildRateLimiting(WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    private static void BuildLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);

            var seqUrl = context.Configuration["Seq:Url"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                config.Enrich.FromLogContext().Enrich.WithEnvironmentUserName().WriteTo.Console().WriteTo.Seq(seqUrl);
            }
        });

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }
}

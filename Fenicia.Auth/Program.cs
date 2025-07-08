namespace Fenicia.Auth;

using System.Text;
using System.Text.Json.Serialization;

using AspNetCoreRateLimit;

using Common;
using Common.Api.Middlewares;
using Common.Externals.Email;

using Contexts;

using Domains.Company.Logic;
using Domains.DataCache;
using Domains.ForgotPassword.Logic;
using Domains.LoginAttempt.Logic;
using Domains.Module.Logic;
using Domains.Order.Logic;
using Domains.RefreshToken.Logic;
using Domains.Role.Logic;
using Domains.Security.Logic;
using Domains.State.Logic;
using Domains.Subscription.Logic;
using Domains.SubscriptionCredit.Logic;
using Domains.Token.Logic;
using Domains.User.Logic;
using Domains.UserRole.Logic;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

using Serilog;

using StackExchange.Redis;

/// <summary>
///     Main program class containing application configuration and startup logic
/// </summary>
public static class Program
{
    /// <summary>
    ///     Application entry point that configures and starts the web application
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var configuration = builder.Configuration;

        Program.BuildLogging(builder);
        Program.BuildRateLimiting(builder, configuration);
        Program.BuildDependencyInjection(builder);
        Program.BuildDatabaseConnection(configuration, builder);
        Program.BuildCors(builder);
        Program.BuildControllers(configuration, builder);

        Program.StartApplication(builder);
    }

    /// <summary>
    ///     Configures and starts the web application with middleware and security settings
    /// </summary>
    /// <param name="builder">The web application builder</param>
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
        // app.UseCsp(opts => opts
        //     .BlockAllMixedContent()
        //     .StyleSources(s => s.Self())
        //     .ScriptSources(s => s.Self())
        //     .FontSources(s => s.Self())
        //     .ImageSources(s => s.Self().CustomSources("data:"))
        //     .DefaultSources(s => s.Self())
        // );

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIpRateLimiting();
        app.MapControllers();

        app.Run();
    }

    /// <summary>
    ///     Configures controllers, authentication, and API behavior
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="builder">The web application builder</param>
    private static void BuildControllers(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var key = Encoding.ASCII.GetBytes(configuration[key: "Jwt:Secret"] ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret));
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
        }).AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AuthProfiles>());

        builder.Services.AddOpenApi();
    }

    /// <summary>
    ///     Configures Cross-Origin Resource Sharing (CORS) policies
    /// </summary>
    /// <param name="builder">The web application builder</param>
    private static void BuildCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "RestrictedCors", policy => { policy.WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });

            options.AddPolicy(name: "DevCors", policy => { policy.WithOrigins("http://localhost:5144", "http://127.0.0.1:5144").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); });
        });
    }

    /// <summary>
    ///     Configures database connection and context
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="builder">The web application builder</param>
    private static void BuildDatabaseConnection(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var connectionString = configuration.GetConnectionString(name: "AuthConnection");

        builder.Services.AddDbContextPool<AuthContext>(x => { x.UseNpgsql(connectionString).EnableSensitiveDataLogging().UseSnakeCaseNamingConvention(); });
    }

    /// <summary>
    ///     Configures dependency injection for services and repositories
    /// </summary>
    /// <param name="builder">The web application builder</param>
    private static void BuildDependencyInjection(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse(configuration: "localhost", ignoreUnknown: true);
            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;
            return ConnectionMultiplexer.Connect(config);
        });

        // Dependency Injection
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

        builder.Services.AddAutoMapper(typeof(Program));
    }

    /// <summary>
    ///     Configures IP-based rate limiting
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <param name="configuration">Application configuration</param>
    private static void BuildRateLimiting(WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        // Rate Limiting setup (AspNetCoreRateLimit)
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection(key: "IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection(key: "IpRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    /// <summary>
    ///     Configures application logging using Serilog
    /// </summary>
    /// <param name="builder">The web application builder</param>
    private static void BuildLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);

            var seqUrl = context.Configuration[key: "Seq:Url"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                config.Enrich.FromLogContext().Enrich.WithEnvironmentUserName().WriteTo.Console().WriteTo.Seq(seqUrl);
            }
        });

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }
}

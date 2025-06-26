using System.Text;
using System.Text.Json.Serialization;

using AspNetCoreRateLimit;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Company.Logic;
using Fenicia.Auth.Domains.DataCache;
using Fenicia.Auth.Domains.ForgotPassword.Logic;
using Fenicia.Auth.Domains.LoginAttempt.Logic;
using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Auth.Domains.Order.Logic;
using Fenicia.Auth.Domains.RefreshToken.Logic;
using Fenicia.Auth.Domains.Role.Logic;
using Fenicia.Auth.Domains.Security.Logic;
using Fenicia.Auth.Domains.Subscription.Logic;
using Fenicia.Auth.Domains.SubscriptionCredit.Logic;
using Fenicia.Auth.Domains.Token.Logic;
using Fenicia.Auth.Domains.User.Logic;
using Fenicia.Auth.Domains.UserRole.Logic;
using Fenicia.Common;
using Fenicia.Common.Api.Middlewares;
using Fenicia.Common.Externals.Email;

using FluentValidation.AspNetCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Scalar.AspNetCore;

using Serilog;

using StackExchange.Redis;

namespace Fenicia.Auth;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, config) =>
        {
            config.ReadFrom.Configuration(context.Configuration);

            var seqUrl = context.Configuration["Seq:Url"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                config.Enrich.FromLogContext()
                    .Enrich.WithEnvironmentUserName()
                    .WriteTo.Console().
                    WriteTo.Seq(seqUrl);
            }
        });

        var configuration = builder.Configuration;

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(TextConstants.InvalidJwtSecret));

        // Rate Limiting setup (AspNetCoreRateLimit)
        builder.Services.AddMemoryCache();
        builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        builder.Services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
        builder.Services.AddInMemoryRateLimiting();
        builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        var connectionString = configuration.GetConnectionString("AuthConnection");

        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse("localhost", true);
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
        builder.Services.AddTransient<IUserRepository, UserRepository>();
        builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();

        builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();

        builder.Services.AddResponseCompression(config =>
        {
            config.EnableForHttps = true;
        });

        builder.Services.AddAutoMapper(typeof(Program));

        builder.Services.AddDbContextPool<AuthContext>(x =>
        {
            x.UseNpgsql(connectionString)
             .EnableSensitiveDataLogging()
             .UseSnakeCaseNamingConvention();
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("RestrictedCors", policy =>
            {
                policy
                    .WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            options.AddPolicy("DevCors", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5144", "http://127.0.0.1:5144")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

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

        builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.AllowTrailingCommas = false;
                x.JsonSerializerOptions.MaxDepth = 0;
                x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            })
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AuthProfiles>());


        builder.Services.AddOpenApi();

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
                x.WithDarkModeToggle(true)
                 .WithTheme(ScalarTheme.Purple)
                 .WithClientButton(true);

                x.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseHttpsRedirection();
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("DevCors");
        }
        else
        {
            app.UseCors("RestrictedCors");
        }

        app.UseHsts();
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(opts => opts.NoReferrer());
        app.UseXXssProtection(options => options.EnabledWithBlockMode());
        app.UseXfo(options => options.Deny());
        app.UseCsp(opts => opts
            .BlockAllMixedContent()
            .StyleSources(s => s.Self())
            .ScriptSources(s => s.Self())
            .FontSources(s => s.Self())
            .ImageSources(s => s.Self().CustomSources("data:"))
            .DefaultSources(s => s.Self())
        );

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIpRateLimiting(); // <- importante: precisa estar aqui
        app.MapControllers();

        app.Run();
    }
}

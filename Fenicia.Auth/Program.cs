using System.Text;
using System.Text.Json.Serialization;

using AspNetCoreRateLimit;

using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.API.Middlewares;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Migrations.Services;
using Fenicia.Externals.Email;

using FluentValidation;
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
        var configBuilder = new ConfigurationManager();
        var commonApiSettingsPath =
            Path.Combine(Directory.GetCurrentDirectory(), "../Fenicia.Common.Api/appsettings.json");

        if (!File.Exists(commonApiSettingsPath))
            throw new FileNotFoundException($"Could not find shared appsettings.json at {commonApiSettingsPath}");

        configBuilder.AddJsonFile(commonApiSettingsPath, false, true);

        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddConfiguration(configBuilder);

        BuildLogging(builder);
        BuildRateLimiting(builder, configBuilder);
        BuildDependencyInjection(builder);
        BuildDatabaseConnection(configBuilder, builder);
        BuildCors(builder);
        BuildControllers(configBuilder, builder);

        StartApplication(builder);
    }

    private static void StartApplication(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<WideEventMiddleware>();
        app.UseResponseCompression();

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(o =>
            {
                o.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer "]
                };
            });
        }

        app.UseHttpsRedirection();
        app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "RestrictedCors");

        app.UseHsts();
        app.UseXContentTypeOptions();
        app.UseReferrerPolicy(o => o.NoReferrer());
        app.UseXXssProtection(o => o.EnabledWithBlockMode());
        app.UseXfo(o => o.Deny());

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseIpRateLimiting();
        app.MapControllers();

        app.Run();
    }

    private static void BuildControllers(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]
                                          ?? throw new InvalidOperationException(TextConstants
                                              .InvalidJwtSecretMessage));
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = true;
            o.ClaimsIssuer = "AuthService";
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.Configure<ApiBehaviorOptions>(o =>
        {
            o.InvalidModelStateResponseFactory = c =>
            {
                var problemDetails = new ValidationProblemDetails(c.ModelState)
                {
                    Type = "https://tools.ietf.org/html/rfc7807",
                    Title = "Um ou mais erros de validação ocorreram.",
                    Status = StatusCodes.Status400BadRequest,
                    Instance = c.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        builder.Services.AddControllers().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.AllowTrailingCommas = false;
            o.JsonSerializerOptions.MaxDepth = 0;
            o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        builder.Services.AddOpenApi();

        builder.Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<BaseModel>();
    }

    private static void BuildCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(o =>
        {
            o.AddPolicy("RestrictedCors",
                policy =>
                {
                    policy.WithOrigins("https://fenicia.gatoninja.com.br", "https://api.fenicia.gatoninja.com.br")
                        .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });

            o.AddPolicy("DevCors",
                policy =>
                {
                    policy.WithOrigins("http://localhost:5144", "http://localhost:3000", "http://localhost:5144",
                        "http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
        });
    }

    private static void BuildDatabaseConnection(ConfigurationManager configuration, WebApplicationBuilder builder)
    {
        var connectionString = configuration.GetConnectionString("Auth");

        builder.Services.AddDbContextPool<AuthContext>(o => o
            .UseNpgsql(connectionString, b => b.MigrationsAssembly("Fenicia.Auth"))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSnakeCaseNamingConvention());
    }

    private static void BuildDependencyInjection(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var config = ConfigurationOptions.Parse("localhost", true);

            config.ConnectRetry = 3;
            config.ConnectTimeout = 5000;

            return ConnectionMultiplexer.Connect(config);
        });

        builder.Services.AddScoped<WideEventContext>();

        builder.Services.AddTransient<IMigrationService, MigrationService>();
        builder.Services.AddTransient<IBrevoProvider, BrevoProvider>();

        builder.Services.AddTransient<CheckCompanyExistsHandler>();

        builder.Services.AddResponseCompression(o => { o.EnableForHttps = true; });
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
                config.Enrich.FromLogContext().Enrich.WithEnvironmentUserName().WriteTo.Console().WriteTo.Seq(seqUrl);
        });

        Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }
}
using System.Security.Claims;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.Commands;
using Fenicia.Auth.Domains.Configuration.Handlers;
using Fenicia.Auth.Domains.Configuration.Responses;

using MediatR;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationControllerTests : IDisposable
{
    private readonly ConfigurationController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public ConfigurationControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        testUserId = Guid.NewGuid();
        var services = new ServiceCollection();
        services.AddSingleton(db);
        services.AddLogging();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<UpsertConfigurationHandler>());

        var provider = services.BuildServiceProvider();
        var sender = provider.GetRequiredService<ISender>();

        mockHttpContext = new Mock<HttpContext>();

        controller = new ConfigurationController(sender) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims(testUserId);
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var companyId = db.CurrentCompanyId ?? Guid.Empty;

        var result = await controller.GetAsync(companyId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Empty(returnedList);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasConfigurations_ReturnsOkWithList()
    {
        var companyId = db.CurrentCompanyId ?? Guid.NewGuid();

        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        db.AuthConfigurations.AddRange(config1, config2);
        await db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetAsync(companyId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Equal(2, returnedList.Count);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {

        var companyId = db.CurrentCompanyId ?? Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var otherUserConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = otherUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        db.AuthConfigurations.AddRange(userConfig, otherUserConfig);
        await db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var result = await controller.GetAsync(companyId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Single(returnedList);
        Assert.Equal(companyId, returnedList[0].CompanyId);
        Assert.Equal("pt-BR", returnedList[0].Value);
    }

    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var companyId = db.CurrentCompanyId ?? Guid.Empty;

        await controller.GetAsync(companyId, wide, ct);

        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.Empty);

        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(configuration);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {

        var companyId = db.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "light"
        };

        db.AuthConfigurations.Add(existingConfig);
        await db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", companyId);

        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var updatedConfig = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(updatedConfig);
        Assert.Equal("pt-BR", updatedConfig.Value);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {

        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", companyId);

        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId, CancellationToken.None);

        Assert.NotNull(configuration);

        Assert.Equal(companyId, configuration.CompanyId);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.Empty);

        await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void ConfigurationController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(ConfigurationController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ConfigurationController_HasRouteAttribute()
    {

        var controllerType = typeof(ConfigurationController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ConfigurationController_HasProducesAttribute()
    {

        var controllerType = typeof(ConfigurationController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}

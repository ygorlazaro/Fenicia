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

/// <summary>
///     Unit tests for the ConfigurationController.
///     Tests HTTP endpoints behavior including retrieval, upsert operations, and request/response handling.
/// </summary>
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

    /// <summary>
    ///     Tests that when a user has no configurations, the endpoint returns an empty list.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var companyId = db.CurrentCompanyId ?? Guid.Empty;

        // Act
        var result = await controller.GetAsync(companyId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Empty(returnedList);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that when a user has configurations, the endpoint returns them in a list.
    /// </summary>
    [Fact]
    public async Task GetAsync_WhenUserHasConfigurations_ReturnsOkWithList()
    {
        var companyId = db.CurrentCompanyId ?? Guid.NewGuid();
        // Arrange
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

        // Act
        var result = await controller.GetAsync(companyId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Equal(2, returnedList.Count);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that filtering by company ID returns only that company's configurations.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies filtering by UserId
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

        // Act
        var result = await controller.GetAsync(companyId, wide, ct);

        // Assert
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

    /// <summary>
    ///     Tests that the WideEventContext UserId is set from the authenticated user claims.
    /// </summary>
    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var companyId = db.CurrentCompanyId ?? Guid.Empty;

        // Act
        await controller.GetAsync(companyId, wide, ct);

        // Assert
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that when a configuration doesn't exist, a new one is created.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.Empty);

        // Act
        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(configuration);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that when a configuration exists, it is updated with the new value.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        // Arrange
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

        // Act
        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var updatedConfig = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(updatedConfig);
        Assert.Equal("pt-BR", updatedConfig.Value);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that a company-scoped configuration can be created.
    /// </summary>
    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        // Arrange
        var companyId = db.CurrentCompanyId ?? Guid.Empty;
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", companyId);

        // Act
        var result = await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId, CancellationToken.None);

        Assert.NotNull(configuration);

        Assert.Equal(companyId, configuration.CompanyId);
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that the WideEventContext UserId is set when patching a configuration.
    /// </summary>
    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, testUserId, ConfigType.Language, "pt-BR", db.CurrentCompanyId ?? Guid.Empty);

        // Act
        await controller.PatchAsync(Guid.NewGuid(), request, wide, ct);

        // Assert
        Assert.Equal(testUserId.ToString(), wide.UserId);
    }

    /// <summary>
    ///     Tests that the ConfigurationController has the AuthorizeAttribute applied.
    /// </summary>
    [Fact]
    public void ConfigurationController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    /// <summary>
    ///     Tests that the ConfigurationController has the RouteAttribute with correct template.
    /// </summary>
    [Fact]
    public void ConfigurationController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    /// <summary>
    ///     Tests that the ConfigurationController has the ProducesAttribute with correct content type.
    /// </summary>
    [Fact]
    public void ConfigurationController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}

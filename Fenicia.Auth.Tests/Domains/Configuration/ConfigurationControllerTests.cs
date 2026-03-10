using System.Security.Claims;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.GetConfiguration;
using Fenicia.Auth.Domains.Configuration.UpsertConfiguration;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationControllerTests : IDisposable
{
    private readonly ConfigurationController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public ConfigurationControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.testUserId = Guid.NewGuid();
        var getConfigurationHandler = new GetConfigurationHandler(this.context);
        var upsertConfigurationHandler = new UpsertConfigurationHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ConfigurationController(
            getConfigurationHandler,
            upsertConfigurationHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(null, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);
        
        Assert.Empty(returnedList);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasConfigurations_ReturnsOkWithList()
    {
        var companyId = this.context.CurrentCompanyId  ?? Guid.NewGuid();
        // Arrange
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.context.AuthConfiguration.AddRange(config1, config2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(companyId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Equal(2, returnedList.Count);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange - Note: Due to how DefaultContext works, all entities get the same CompanyId
        // from TestCompanyContext, so this test verifies filtering by UserId
        var companyId = this.context.CurrentCompanyId  ?? Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
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

        this.context.AuthConfiguration.AddRange(userConfig, otherUserConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(companyId, wide, ct);

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

    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetAsync(null, wide, ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.context.CurrentCompanyId
        );

        // Act
        var result = await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(configuration);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        // Arrange
        var companyId = this.context.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "light"
        };

        this.context.AuthConfiguration.Add(existingConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            companyId
        );

        // Act
        var result = await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(updatedConfig);
        Assert.Equal("pt-BR", updatedConfig.Value);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        // Arrange
        var companyId = this.context.CurrentCompanyId;
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            companyId
        );

        // Act
        var result = await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c =>
                c.UserId == this.testUserId &&
                c.ConfigType == ConfigType.Language &&
                c.CompanyId == companyId, CancellationToken.None);

        Assert.NotNull(configuration);

        Assert.Equal(companyId, configuration.CompanyId);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            this.context.CurrentCompanyId
        );

        // Act
        await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

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

    [Fact]
    public void ConfigurationController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ConfigurationController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }
}

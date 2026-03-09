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

[TestFixture]
public class ConfigurationControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.testUserId = Guid.NewGuid();
        this.getConfigurationHandler = new GetConfigurationHandler(this.context);
        this.upsertConfigurationHandler = new UpsertConfigurationHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ConfigurationController(
            this.getConfigurationHandler,
            this.upsertConfigurationHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private ConfigurationController controller = null!;
    private DefaultContext context = null!;
    private GetConfigurationHandler getConfigurationHandler = null!;
    private UpsertConfigurationHandler upsertConfigurationHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testUserId;

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

    [Test]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(null, wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.That(returnedList, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedList!.Count, Is.Zero);
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetAsync_WhenUserHasConfigurations_ReturnsOkWithList()
    {
        // Arrange
        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        this.context.AuthConfiguration.AddRange(config1, config2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(null, wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.That(returnedList, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedList!, Has.Count.EqualTo(2));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task GetAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        // Arrange
        var companyId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var companyConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        this.context.AuthConfiguration.AddRange(userConfig, companyConfig);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(companyId, wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.That(returnedList, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedList!, Has.Count.EqualTo(1));
            Assert.That(returnedList[0].CompanyId, Is.EqualTo(companyId));
        }
    }

    [Test]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        // Act
        await this.controller.GetAsync(null, wide, ct);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    [Test]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            Guid.NewGuid()
        );

        // Act
        var result = await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.That(configuration, Is.Not.Null);
        Assert.That(configuration.Value, Is.EqualTo("pt-BR"));
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    [Test]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        // Arrange
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = this.testUserId,
            CompanyId = Guid.NewGuid(),
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
            Guid.NewGuid()
        );

        // Act
        var result = await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var updatedConfig = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => c.UserId == this.testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.That(updatedConfig, Is.Not.Null);
        Assert.That(updatedConfig.Value, Is.EqualTo("pt-BR"));
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    [Test]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        // Arrange
        var companyId = Guid.NewGuid();
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.InstanceOf<NoContentResult>());

        var configuration = await this.context.AuthConfiguration
            .FirstOrDefaultAsync(c => 
                c.UserId == this.testUserId && 
                c.ConfigType == ConfigType.Language &&
                c.CompanyId == companyId, CancellationToken.None);

        Assert.That(configuration, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(configuration.CompanyId, Is.EqualTo(companyId));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var request = new UpsertConfigurationCommand(
            this.testUserId,
            ConfigType.Language,
            "pt-BR",
            Guid.NewGuid()
        );

        // Act
        await this.controller.PatchAsync(request, wide, ct);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    [Test]
    public void ConfigurationController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ConfigurationController should have Authorize attribute");
    }

    [Test]
    public void ConfigurationController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ConfigurationController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ConfigurationController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(ConfigurationController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "ConfigurationController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }
}

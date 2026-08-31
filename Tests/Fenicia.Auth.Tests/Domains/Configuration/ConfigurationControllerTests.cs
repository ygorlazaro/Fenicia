using System.Security.Claims;

using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationControllerTests : IDisposable
{
    private readonly ConfigurationController _controller;
    private readonly DefaultContext _db;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ConfigurationControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _testUserId = Guid.NewGuid();

        _mockHttpContext = new Mock<HttpContext>();

        var configurationRepository = new ConfigurationRepository(_db);
        var configurationService = new ConfigurationService(configurationRepository);
        _controller = new ConfigurationController(configurationService) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        var result = await _controller.GetAsync(companyId, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Empty(returnedList);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasConfigurations_ReturnsOkWithList()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.NewGuid();

        var config1 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        var config2 = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "en-US"
        };

        _db.AuthConfigurations.AddRange(config1, config2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetAsync(companyId, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Equal(2, returnedList.Count);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_WithCompanyIdFilter_ReturnsOnlyCompanyConfigurations()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
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

        _db.AuthConfigurations.AddRange(userConfig, otherUserConfig);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetAsync(companyId, wide, cancellationToken);

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
        var cancellationToken = CancellationToken.None;
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;

        await _controller.GetAsync(companyId, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var result = await _controller.PatchAsync(Guid.NewGuid(), _db.CurrentCompanyId ?? Guid.Empty, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(configuration);
        Assert.Equal("pt-BR", configuration.Value);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.NewGuid();
        var existingConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "light"
        };

        _db.AuthConfigurations.Add(existingConfig);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var updatedConfig = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language, CancellationToken.None);

        Assert.NotNull(updatedConfig);
        Assert.Equal("pt-BR", updatedConfig.Value);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        var companyId = _db.CurrentCompanyId ?? Guid.Empty;
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);

        var configuration = await _db.AuthConfigurations.FirstOrDefaultAsync(c => c.UserId == _testUserId && c.ConfigType == ConfigType.Language && c.CompanyId == companyId, CancellationToken.None);

        Assert.NotNull(configuration);

        Assert.Equal(companyId, configuration.CompanyId);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        await _controller.PatchAsync(Guid.NewGuid(), _db.CurrentCompanyId ?? Guid.Empty, request, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
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

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

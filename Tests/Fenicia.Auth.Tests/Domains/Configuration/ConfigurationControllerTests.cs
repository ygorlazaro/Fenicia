using System.Security.Claims;
using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.DTOs;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Configuration;

public class ConfigurationControllerTests
{
    private readonly ConfigurationController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Mock<IConfigurationService> _mockService;

    public ConfigurationControllerTests()
    {
        _testUserId = Guid.NewGuid();

        _mockHttpContext = new Mock<HttpContext>();
        _mockService = new Mock<IConfigurationService>();

        _controller = new ConfigurationController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

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
        var companyId = Guid.NewGuid();

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

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                .. ((List<ConfigurationModel>)[config1, config2]).Select(c => c.MapToGetConfigurationResponse())
            ]);

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
        var companyId = Guid.NewGuid();

        var userConfig = new ConfigurationModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            CompanyId = companyId,
            ConfigType = ConfigType.Language,
            Value = "pt-BR"
        };

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([.. ((List<ConfigurationModel>)[userConfig]).Select(c => c.MapToGetConfigurationResponse())]);

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
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        await _controller.GetAsync(companyId, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        _mockService.Setup(s => s.UpsertAsync(It.IsAny<UpsertConfigurationCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.PatchAsync(Guid.NewGuid(), Guid.NewGuid(), request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.UpsertAsync(It.IsAny<UpsertConfigurationCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        var companyId = Guid.NewGuid();
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        _mockService.Setup(s => s.UpsertAsync(It.IsAny<UpsertConfigurationCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        _mockService.Setup(s => s.UpsertAsync(It.IsAny<UpsertConfigurationCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _controller.PatchAsync(Guid.NewGuid(), Guid.NewGuid(), request, wide, cancellationToken);

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

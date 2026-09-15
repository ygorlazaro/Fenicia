using System.Security.Claims;
using Fenicia.Auth.Domains.Configuration;
using Fenicia.Auth.Domains.Configuration.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Configuration;
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
    private readonly Mock<IConfigurationService> _mockService;
    private readonly Guid _testUserId;

    public ConfigurationControllerTests()
    {
        _testUserId = Guid.NewGuid();

        _mockHttpContext = new Mock<HttpContext>();
        _mockService = new Mock<IConfigurationService>();

        _controller = new ConfigurationController(_mockService.Object)
        { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    [Fact]
    public async Task GetAsync_WhenUserHasNoConfigurations_ReturnsOkWithEmptyList()
    {
        var cancellationToken = CancellationToken.None;
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _controller.GetAsync(companyId, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Empty(returnedList);
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

        var response1 = new GetConfigurationResponse(config1.Id, config1.UserId, config1.CompanyId, config1.ConfigType, config1.Value);
        var response2 = new GetConfigurationResponse(config2.Id, config2.UserId, config2.CompanyId, config2.ConfigType, config2.Value);

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([response1, response2]);

        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetAsync(companyId, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedList = okResult.Value as List<GetConfigurationResponse>;
        Assert.NotNull(returnedList);

        Assert.Equal(2, returnedList.Count);
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

        var response = new GetConfigurationResponse(userConfig.Id, userConfig.UserId, userConfig.CompanyId, userConfig.ConfigType, userConfig.Value);

        _mockService.Setup(s => s.GetAllAsync(_testUserId, companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([response]);

        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetAsync(companyId, cancellationToken);

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
    public async Task PatchAsync_WhenConfigurationDoesNotExist_CreatesNewConfiguration()
    {
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        _mockService.Setup(s => s.UpsertAsync(
                It.IsAny<UpsertConfigurationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.PatchAsync(Guid.NewGuid(), Guid.NewGuid(), request, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PatchAsync_WhenConfigurationExists_UpdatesExistingConfiguration()
    {
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.UpsertAsync(
                It.IsAny<UpsertConfigurationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task PatchAsync_WithCompanyId_CreatesCompanyConfiguration()
    {
        var companyId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        var request = new UpsertConfigurationCommand(null, _testUserId, ConfigType.Language, "pt-BR");

        _mockService.Setup(s => s.UpsertAsync(
                It.IsAny<UpsertConfigurationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.PatchAsync(Guid.NewGuid(), companyId, request, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NoContentResult>(result);
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

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ConfigurationController_HasProducesAttribute()
    {
        var controllerType = typeof(ConfigurationController);

        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

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

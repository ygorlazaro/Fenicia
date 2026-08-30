using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class DataSourceControllerTests : IDisposable
{
    private readonly DataSourceController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<DataSourceService> _mockService;

    public DataSourceControllerTests()
    {
        _mockService = new Mock<DataSourceService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new DataSourceController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetPositionsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetAllPositionForDataSourceResponse>());

        _mockService.Setup(s => s.GetProductCategoriesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetAllProductCategoryForDataSourceResponse>());
    }

    [Fact]
    public async Task GetPositionsAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetPositionsAsync(wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetProductCategoriesAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetProductCategoriesAsync(wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
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

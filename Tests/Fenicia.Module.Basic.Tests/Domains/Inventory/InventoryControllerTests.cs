using System.Security.Claims;

using AwesomeAssertions;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Inventory.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryControllerTests : IDisposable
{
    private readonly InventoryController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IInventoryService> _mockService;

    public InventoryControllerTests()
    {
        _mockService = new Mock<IInventoryService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new InventoryController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryDashboardAsync(wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryHealthAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryHealthAsync(wide, 90, 3.0, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryByProductIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetDashboardAsync(It.IsAny<GetInventoryDashboardQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryDashboardResponse());

        _mockService.Setup(s => s.GetHealthAsync(It.IsAny<GetInventoryHealthQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryHealthResponse());

        _mockService.Setup(s => s.GetAsync(It.IsAny<GetInventoryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryResponse());

        _mockService.Setup(s => s.GetByProductAsync(It.IsAny<GetInventoryByProductQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new InventoryResponse());
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

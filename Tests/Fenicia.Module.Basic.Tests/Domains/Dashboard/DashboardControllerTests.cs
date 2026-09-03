using System.Security.Claims;
using AwesomeAssertions;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class DashboardControllerTests : IDisposable
{
    private readonly DashboardController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IDashboardService> _mockService;

    public DashboardControllerTests()
    {
        _mockService = new Mock<IDashboardService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new DashboardController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetFinancialDashboardAsync(wide, 90, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetFinancialDashboardAsync(
                It.IsAny<GetFinancialDashboardQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FinancialDashboardResponse());
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
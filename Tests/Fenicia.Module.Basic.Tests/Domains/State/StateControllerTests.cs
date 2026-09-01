using System.Security.Claims;

using AwesomeAssertions;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class StateControllerTests : IDisposable
{
    private readonly StateController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IStateService> _mockService;

    public StateControllerTests()
    {
        _mockService = new Mock<IStateService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new StateController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAllAsync(wide, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllStateQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
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

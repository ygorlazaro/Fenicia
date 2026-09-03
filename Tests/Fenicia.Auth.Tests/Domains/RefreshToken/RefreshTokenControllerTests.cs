using System.Security.Claims;
using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class RefreshTokenControllerTests
{
    private readonly RefreshTokenController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public RefreshTokenControllerTests()
    {
        _testUserId = Guid.NewGuid();
        _mockHttpContext = new Mock<HttpContext>();
        var mockService = new Mock<IRefreshTokenService>();

        _controller = new RefreshTokenController(mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    [Fact]
    public void RefreshTokenController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(RefreshTokenController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void RefreshTokenController_HasRouteAttribute()
    {
        var controllerType = typeof(RefreshTokenController);

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void RefreshTokenController_HasProducesAttribute()
    {
        var controllerType = typeof(RefreshTokenController);

        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public async Task PostAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        await _controller.PostAsync(wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        await _controller.GetAsync("some_token", wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task PatchAsync_SetsWideEventContextUserId()
    {
        var command = new UpdateRefreshTokenCommand(true);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        await _controller.PatchAsync("some_token", command, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
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
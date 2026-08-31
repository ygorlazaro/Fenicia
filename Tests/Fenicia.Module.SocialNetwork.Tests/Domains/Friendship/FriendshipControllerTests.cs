using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Friendship;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Friendship;

public class FriendshipControllerTests : IDisposable
{
    private readonly FriendshipController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<FriendshipService> _mockService;

    public FriendshipControllerTests()
    {
        _mockService = new Mock<FriendshipService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new FriendshipController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task FollowAsync_WhenValid_ReturnsCreated()
    {
        var command = new FollowCommand(Guid.NewGuid());
        var wide = new WideEventContext();

        var result = await _controller.FollowAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task UnfollowAsync_WhenValid_ReturnsNoContent()
    {
        var wide = new WideEventContext();

        var result = await _controller.UnfollowAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetFollowersAsync_WhenValid_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetFollowersAsync(Guid.NewGuid(), wide, 1, 10, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetFollowingAsync_WhenValid_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetFollowingAsync(Guid.NewGuid(), wide, 1, 10, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task IsFollowingAsync_WhenValid_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.IsFollowingAsync(Guid.NewGuid(), Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.FollowAsync(It.IsAny<FollowCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FollowCommand cmd, Guid userId, CancellationToken cancellationToken) => new AddFriendshipResponse(Guid.NewGuid(), userId, cmd.TargetUserId, DateTime.UtcNow, true));

        _mockService.Setup(s => s.GetFollowersAsync(It.IsAny<GetFollowersQuery>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetFollowersResponse>>(new List<GetFollowersResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.GetFollowingAsync(It.IsAny<GetFollowingQuery>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetFollowingResponse>>(new List<GetFollowingResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.IsFollowingAsync(It.IsAny<IsFollowingQuery>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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

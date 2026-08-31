using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Block;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Block;

public class BlockControllerTests : IDisposable
{
    private readonly BlockController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<BlockService> _mockService;

    public BlockControllerTests()
    {
        _mockService = new Mock<BlockService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new BlockController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task BlockAsync_WhenValid_ReturnsCreated()
    {
        var command = new BlockCommand(Guid.NewGuid());
        var wide = new WideEventContext();

        var result = await _controller.BlockAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task UnblockAsync_WhenValid_ReturnsNoContent()
    {
        var wide = new WideEventContext();

        var result = await _controller.UnblockAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetBlockedAsync_WhenValid_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetBlockedAsync(Guid.NewGuid(), wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task IsBlockedAsync_WhenValid_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.IsBlockedAsync(Guid.NewGuid(), Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.BlockAsync(It.IsAny<BlockCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((BlockCommand cmd, Guid userId, CancellationToken cancellationToken) => new AddBlockResponse(Guid.NewGuid(), userId, cmd.BlockedUserId, DateTime.UtcNow, null, true));

        _mockService.Setup(s => s.GetBlockedAsync(It.IsAny<GetBlockedQuery>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetBlockedResponse>>(new List<GetBlockedResponse>(), 0, 1, 10));

        _mockService.Setup(s => s.IsBlockedAsync(It.IsAny<IsBlockedQuery>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
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

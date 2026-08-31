using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Profile;

public class ProfileControllerTests : IDisposable
{
    private readonly ProfileController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ProfileService> _mockService;

    public ProfileControllerTests()
    {
        _mockService = new Mock<ProfileService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProfileController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenProfileExists_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetAsync(wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileExists_ReturnsOk()
    {
        var wide = new WideEventContext();

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetProfileByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProfileByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProfileExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Bio", null, null, null, null, null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProfileDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Bio", null, null, null, null, null);

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateProfileCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProfileResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProfileByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProfileByIdQuery q, CancellationToken cancellationToken) => new GetProfileByIdResponse(q.Id, Guid.NewGuid(), "Bio", null, null, null, null, null));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProfileCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProfileCommand cmd, Guid userId, CancellationToken cancellationToken) => new UpdateProfileResponse(cmd.Id, userId, cmd.Bio, cmd.ImageUrl, cmd.Website, cmd.Location, cmd.Phone, cmd.BirthDate));
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

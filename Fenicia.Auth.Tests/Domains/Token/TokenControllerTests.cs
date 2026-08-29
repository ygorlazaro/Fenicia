using Bogus;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.Token;

public class TokenControllerTests
{
    private readonly TokenController _controller;
    private readonly Faker _faker = new();
    private readonly Mock<TokenService> _tokenServiceMock;
    private readonly Mock<RefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<UserService> _userServiceMock;

    public TokenControllerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(new Mock<IDatabase>().Object);

        _tokenServiceMock = new Mock<TokenService>(null!, null!, null!, null!) { CallBase = true };
        _refreshTokenServiceMock = new Mock<RefreshTokenService>(new RefreshTokenRepository(redisMock.Object)) { CallBase = true };
        _userServiceMock = new Mock<UserService>(null!, null!, null!, null!, null!) { CallBase = true };

        _controller = new TokenController(_tokenServiceMock.Object, _refreshTokenServiceMock.Object, _userServiceMock.Object);
    }

    [Fact]
    public async Task PostAsync_WhenInvalidCredentials_ReturnsBadRequest()
    {
        var request = new GenerateTokenQuery(_faker.Internet.Email(), _faker.Internet.Password());
        var wide = new WideEventContext();

        _tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PermissionDeniedException("Invalid username or password."));

        var result = await _controller.PostAsync(request, wide, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WhenValidCredentials_ReturnsCreated()
    {
        var request = new GenerateTokenQuery(_faker.Internet.Email(), _faker.Internet.Password());
        var wide = new WideEventContext();
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, request.Email);

        _tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _tokenServiceMock.Setup(s => s.GenerateString(It.IsAny<GenerateTokenResponse>())).Returns("jwt");
        _refreshTokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync("refresh");

        var result = await _controller.PostAsync(request, wide, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public async Task Refresh_WhenInvalidRefreshToken_ReturnsBadRequest()
    {
        var request = new ValidateTokenQuery(Guid.NewGuid(), "invalid");
        var wide = new WideEventContext();

        _refreshTokenServiceMock.Setup(s => s.ValidateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await _controller.Refresh(request, wide, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Refresh_WhenValidRefreshToken_ReturnsCreated()
    {
        var userId = Guid.NewGuid();
        var request = new ValidateTokenQuery(userId, "refresh");
        var wide = new WideEventContext();
        var user = new GetUserForRefreshResponse(userId, _faker.Internet.Email(), _faker.Person.FullName);

        _refreshTokenServiceMock.Setup(s => s.ValidateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _refreshTokenServiceMock.Setup(s => s.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _userServiceMock.Setup(s => s.GetForRefreshAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _tokenServiceMock.Setup(s => s.GenerateString(It.IsAny<GenerateTokenResponse>())).Returns("jwt");
        _refreshTokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync("refresh2");

        var result = await _controller.Refresh(request, wide, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(userId.ToString(), wide.UserId);
    }

    [Fact]
    public void TokenController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(TokenController);
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}

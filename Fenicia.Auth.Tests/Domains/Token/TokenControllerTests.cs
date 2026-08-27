using Bogus;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs.Queries;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs.Queries;
using Fenicia.Auth.Domains.Token.DTOs.Responses;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs.Queries;
using Fenicia.Auth.Domains.User.DTOs.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.Token;

public class TokenControllerTests
{
    private readonly TokenController controller;
    private readonly Faker faker = new();
    private readonly Mock<TokenService> tokenServiceMock;
    private readonly Mock<RefreshTokenService> refreshTokenServiceMock;
    private readonly Mock<UserService> userServiceMock;

    public TokenControllerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(new Mock<IDatabase>().Object);

        tokenServiceMock = new Mock<TokenService>(null!, null!, null!) { CallBase = true };
        refreshTokenServiceMock = new Mock<RefreshTokenService>(redisMock.Object) { CallBase = true };
        userServiceMock = new Mock<UserService>(null!) { CallBase = true };

        controller = new TokenController(tokenServiceMock.Object, refreshTokenServiceMock.Object, userServiceMock.Object);
    }

    [Fact]
    public async Task PostAsync_WhenInvalidCredentials_ReturnsBadRequest()
    {
        var request = new GenerateTokenQuery(faker.Internet.Email(), faker.Internet.Password());
        var wide = new WideEventContext();

        tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PermissionDeniedException("Invalid username or password."));

        var result = await controller.PostAsync(request, wide, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WhenValidCredentials_ReturnsCreated()
    {
        var request = new GenerateTokenQuery(faker.Internet.Email(), faker.Internet.Password());
        var wide = new WideEventContext();
        var user = new GenerateTokenResponse(Guid.NewGuid(), faker.Person.FullName, request.Email);

        tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        tokenServiceMock.Setup(s => s.GenerateString(It.IsAny<GenerateTokenResponse>())).Returns("jwt");
        refreshTokenServiceMock.Setup(s => s.Generate(It.IsAny<Guid>())).Returns("refresh");

        var result = await controller.PostAsync(request, wide, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public async Task Refresh_WhenInvalidRefreshToken_ReturnsBadRequest()
    {
        var request = new ValidateTokenQuery(Guid.NewGuid(), "invalid");
        var wide = new WideEventContext();

        refreshTokenServiceMock.Setup(s => s.ValidateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await controller.Refresh(request, wide, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Refresh_WhenValidRefreshToken_ReturnsCreated()
    {
        var userId = Guid.NewGuid();
        var request = new ValidateTokenQuery(userId, "refresh");
        var wide = new WideEventContext();
        var user = new GetUserForRefreshResponse(userId, faker.Internet.Email(), faker.Person.FullName);

        refreshTokenServiceMock.Setup(s => s.ValidateAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        refreshTokenServiceMock.Setup(s => s.InvalidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        userServiceMock.Setup(s => s.GetForRefreshAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        tokenServiceMock.Setup(s => s.GenerateString(It.IsAny<GenerateTokenResponse>())).Returns("jwt");
        refreshTokenServiceMock.Setup(s => s.Generate(It.IsAny<Guid>())).Returns("refresh2");

        var result = await controller.Refresh(request, wide, CancellationToken.None);

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

using Bogus;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.Interfaces;
using Fenicia.Common.DTOs.Auth.Token;
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
    private readonly Mock<ITokenService> _tokenServiceMock;

    public TokenControllerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(new Mock<IDatabase>().Object);

        _tokenServiceMock = new Mock<ITokenService>(MockBehavior.Strict) { CallBase = true };

        _controller = new TokenController(_tokenServiceMock.Object, new TokenMapper());
    }

    [Fact]
    public async Task PostAsync_WhenInvalidCredentials_ReturnsBadRequest()
    {
        var request = new GenerateTokenQuery { Email = _faker.Internet.Email(), Password = _faker.Internet.Password() };

        _tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new PermissionDeniedException("Invalid username or password."));

        var result = await _controller.PostAsync(request, CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WhenValidCredentials_ReturnsCreated()
    {
        var request = new GenerateTokenQuery { Email = _faker.Internet.Email(), Password = _faker.Internet.Password() };
        var user = new GenerateTokenResponse { Id = Guid.NewGuid(), Name = _faker.Person.FullName, Email = request.Email };

        _tokenServiceMock.Setup(s => s.GenerateAsync(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _tokenServiceMock.Setup(s => s.GenerateString(It.IsAny<GenerateTokenResponse>())).Returns("jwt");

        var result = await _controller.PostAsync(request, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public void TokenController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(TokenController);
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}

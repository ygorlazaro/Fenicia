using Bogus;

using Fenicia.Auth.Domains.RefreshToken.Queries;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Auth.Domains.Token.Responses;
using Fenicia.Auth.Domains.User.Queries;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Domains.Token;

public class TokenControllerTests
{
    private readonly TokenController controller;
    private readonly Faker faker = new();
    private readonly Mock<ISender> sender = new();

    public TokenControllerTests()
    {
        controller = new TokenController(sender.Object);
    }

    [Fact]
    public async Task PostAsync_WhenInvalidCredentials_ReturnsBadRequest()
    {
        var request = new GenerateTokenQuery(faker.Internet.Email(), faker.Internet.Password());
        var wide = new WideEventContext();

        sender.Setup(s => s.Send(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>()))
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

        sender.Setup(s => s.Send(It.IsAny<GenerateTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        sender.Setup(s => s.Send(It.IsAny<GenerateTokenStringQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync("jwt");
        sender.Setup(s => s.Send(It.IsAny<Fenicia.Auth.Domains.RefreshToken.Commands.GenerateRefreshTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("refresh");

        var result = await controller.PostAsync(request, wide, CancellationToken.None);

        Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(request.Email, wide.UserId);
    }

    [Fact]
    public async Task Refresh_WhenInvalidRefreshToken_ReturnsBadRequest()
    {
        var request = new ValidateTokenQuery(Guid.NewGuid(), "invalid");
        var wide = new WideEventContext();

        sender.Setup(s => s.Send(It.IsAny<ValidateTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

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

        sender.Setup(s => s.Send(It.IsAny<ValidateTokenQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        sender.Setup(s => s.Send(It.IsAny<Fenicia.Auth.Domains.RefreshToken.Commands.InvalidateRefreshTokenCommand>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        sender.Setup(s => s.Send(It.IsAny<GetUserForRefreshQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        sender.Setup(s => s.Send(It.IsAny<GenerateTokenStringQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync("jwt");
        sender.Setup(s => s.Send(It.IsAny<Fenicia.Auth.Domains.RefreshToken.Commands.GenerateRefreshTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("refresh2");

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

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Mvc;

using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class TokenControllerTests
{
    private Mock<ITokenService> tokenServiceMock;
    private Mock<IRefreshTokenService> refreshServiceMock;
    private Mock<IUserService> userServiceMock;
    private TokenController sut;

    [SetUp]
    public void Setup()
    {
        tokenServiceMock = new Mock<ITokenService>();
        refreshServiceMock = new Mock<IRefreshTokenService>();
        userServiceMock = new Mock<IUserService>();

        sut = new TokenController(tokenServiceMock.Object, refreshServiceMock.Object, userServiceMock.Object);
    }

    [Test]
    public async Task PostAsync_ReturnsTokenResponse_WhenUserValid()
    {
        var request = new TokenRequest { Email = "a@b.com", Password = "p" };
        var user = new UserResponse { Id = Guid.NewGuid(), Email = request.Email, Name = "N" };

        userServiceMock.Setup(x => x.GetForLoginAsync(request, CancellationToken.None)).ReturnsAsync(user);
        tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns("tok");
        refreshServiceMock.Setup(x => x.GenerateRefreshToken(user.Id)).Returns("ref");

        var result = await sut.PostAsync(request, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var tr = ok.Value as TokenResponse;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tr?.AccessToken, Is.EqualTo("tok"));
            Assert.That(tr?.RefreshToken, Is.EqualTo("ref"));
        }
    }

    [Test]
    public async Task PostAsync_ReturnsBadRequest_OnPermissionDenied()
    {
        var request = new TokenRequest { Email = "a@b.com", Password = "p" };
        userServiceMock.Setup(x => x.GetForLoginAsync(request, CancellationToken.None)).ThrowsAsync(new PermissionDeniedException("bad"));

        var result = await sut.PostAsync(request, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        var bad = result.Result as BadRequestObjectResult;
        Assert.That(bad, Is.Not.Null);
    }

    [Test]
    public async Task Refresh_ReturnsBadRequest_WhenTokenInvalid()
    {
        var req = new RefreshTokenRequest { UserId = Guid.NewGuid(), RefreshToken = "r" };
        refreshServiceMock.Setup(x => x.ValidateTokenAsync(req.UserId, req.RefreshToken, CancellationToken.None)).ReturnsAsync(false);

        var result = await sut.Refresh(req, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Refresh_InvalidatesAndReturnsToken_WhenValid()
    {
        var userId = Guid.NewGuid();
        var req = new RefreshTokenRequest { UserId = userId, RefreshToken = "r" };

        refreshServiceMock.Setup(x => x.ValidateTokenAsync(userId, req.RefreshToken, CancellationToken.None)).ReturnsAsync(true);
        refreshServiceMock.Setup(x => x.InvalidateRefreshTokenAsync(req.RefreshToken, CancellationToken.None)).Returns(Task.CompletedTask);

        var user = new UserResponse { Id = userId, Email = "x@x.com", Name = "X" };
        userServiceMock.Setup(x => x.GetUserForRefreshAsync(userId, CancellationToken.None)).ReturnsAsync(user);

        tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns("tok");
        refreshServiceMock.Setup(x => x.GenerateRefreshToken(userId)).Returns("newref");

        var result = await sut.Refresh(req, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        var tr = ok?.Value as TokenResponse;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tr?.AccessToken, Is.EqualTo("tok"));
            Assert.That(tr?.RefreshToken, Is.EqualTo("newref"));
        }

        refreshServiceMock.Verify(x => x.ValidateTokenAsync(userId, req.RefreshToken, CancellationToken.None), Times.Once);
        refreshServiceMock.Verify(x => x.InvalidateRefreshTokenAsync(req.RefreshToken, CancellationToken.None), Times.Once);
    }
}

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
    private Mock<IRefreshTokenService> refreshServiceMock;
    private TokenController sut;
    private Mock<ITokenService> tokenServiceMock;
    private Mock<IUserService> userServiceMock;

    [SetUp]
    public void Setup()
    {
        this.tokenServiceMock = new Mock<ITokenService>();
        this.refreshServiceMock = new Mock<IRefreshTokenService>();
        this.userServiceMock = new Mock<IUserService>();

        this.sut = new TokenController(this.tokenServiceMock.Object, this.refreshServiceMock.Object,
            this.userServiceMock.Object);
    }

    [Test]
    public async Task PostAsync_ReturnsTokenResponse_WhenUserValid()
    {
        var request = new TokenRequest { Email = "a@b.com", Password = "p" };
        var user = new UserResponse { Id = Guid.NewGuid(), Email = request.Email, Name = "N" };

        this.userServiceMock.Setup(x => x.GetForLoginAsync(request, CancellationToken.None)).ReturnsAsync(user);
        this.tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns("tok");
        this.refreshServiceMock.Setup(x => x.GenerateRefreshToken(user.Id)).Returns("ref");

        var result = await this.sut.PostAsync(request, new WideEventContext(), CancellationToken.None);

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
        this.userServiceMock.Setup(x => x.GetForLoginAsync(request, CancellationToken.None))
            .ThrowsAsync(new PermissionDeniedException("bad"));

        var result = await this.sut.PostAsync(request, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
        var bad = result.Result as BadRequestObjectResult;
        Assert.That(bad, Is.Not.Null);
    }

    [Test]
    public async Task Refresh_ReturnsBadRequest_WhenTokenInvalid()
    {
        var req = new RefreshTokenRequest { UserId = Guid.NewGuid(), RefreshToken = "r" };
        this.refreshServiceMock.Setup(x => x.ValidateTokenAsync(req.UserId, req.RefreshToken, CancellationToken.None))
            .ReturnsAsync(false);

        var result = await this.sut.Refresh(req, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Refresh_InvalidatesAndReturnsToken_WhenValid()
    {
        var userId = Guid.NewGuid();
        var req = new RefreshTokenRequest { UserId = userId, RefreshToken = "r" };

        this.refreshServiceMock.Setup(x => x.ValidateTokenAsync(userId, req.RefreshToken, CancellationToken.None))
            .ReturnsAsync(true);
        this.refreshServiceMock.Setup(x => x.InvalidateRefreshTokenAsync(req.RefreshToken, CancellationToken.None))
            .Returns(Task.CompletedTask);

        var user = new UserResponse { Id = userId, Email = "x@x.com", Name = "X" };
        this.userServiceMock.Setup(x => x.GetUserForRefreshAsync(userId, CancellationToken.None)).ReturnsAsync(user);

        this.tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns("tok");
        this.refreshServiceMock.Setup(x => x.GenerateRefreshToken(userId)).Returns("newref");

        var result = await this.sut.Refresh(req, new WideEventContext(), CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        var tr = ok?.Value as TokenResponse;
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tr?.AccessToken, Is.EqualTo("tok"));
            Assert.That(tr?.RefreshToken, Is.EqualTo("newref"));
        }

        this.refreshServiceMock.Verify(x => x.ValidateTokenAsync(userId, req.RefreshToken, CancellationToken.None),
            Times.Once);
        this.refreshServiceMock.Verify(x => x.InvalidateRefreshTokenAsync(req.RefreshToken, CancellationToken.None),
            Times.Once);
    }
}
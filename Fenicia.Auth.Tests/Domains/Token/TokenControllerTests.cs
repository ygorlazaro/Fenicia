using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;
using Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.ValidateToken;
using Fenicia.Auth.Domains.Security.VerifyPassword;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.GenerateToken;
using Fenicia.Auth.Domains.Token.GenerateTokenString;
using Fenicia.Auth.Domains.User.GetByEmail;
using Fenicia.Auth.Domains.User.GetUserForRefresh;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.Token;

[TestFixture]
public class TokenControllerTests
{
    private Auth.Domains.Token.TokenController controller = null!;
    private AuthContext context = null!;
    private GenerateTokenHandler generateTokenHandler = null!;
    private GenerateTokenStringHandler generateTokenStringHandler = null!;
    private GenerateRefreshTokenHandler generateRefreshTokenHandler = null!;
    private ValidateTokenHandler validateTokenHandler = null!;
    private InvalidateRefreshTokenHandler invalidateRefreshTokenHandler = null!;
    private GetUserForRefreshHandler getUserForRefreshHandler = null!;
    private Mock<LoginAttemptHandler> mockLoginAttemptHandler = null!;
    private Mock<GetByEmailHandler> mockGetByEmailHandler = null!;
    private Mock<IncrementAttempts> mockIncrementAttempts = null!;
    private Mock<VerifyPasswordHandler> mockVerifyPasswordHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Mock<IConfiguration> mockConfiguration = null!;
    private Mock<IConnectionMultiplexer> mockRedis = null!;
    private Guid testUserId;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.testUserId = Guid.NewGuid();
        var cache = new MemoryCache(new MemoryCacheOptions());

        this.mockLoginAttemptHandler = new Mock<LoginAttemptHandler>(cache);
        this.mockGetByEmailHandler = new Mock<GetByEmailHandler>(this.context);
        this.mockIncrementAttempts = new Mock<IncrementAttempts>(cache);
        this.mockVerifyPasswordHandler = new Mock<VerifyPasswordHandler>();
        this.mockConfiguration = new Mock<IConfiguration>();
        this.mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("ThisIsASecretKeyForJwtSigning123456");
        this.mockRedis = new Mock<IConnectionMultiplexer>();
        var mockDatabase = new Mock<IDatabase>();
        this.mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(mockDatabase.Object);

        this.generateTokenHandler = new GenerateTokenHandler(
            this.mockLoginAttemptHandler.Object,
            this.mockGetByEmailHandler.Object,
            this.mockIncrementAttempts.Object,
            this.mockVerifyPasswordHandler.Object);

        this.generateTokenStringHandler = new GenerateTokenStringHandler(this.mockConfiguration.Object);
        this.generateRefreshTokenHandler = new GenerateRefreshTokenHandler(this.mockRedis.Object);
        this.validateTokenHandler = new ValidateTokenHandler(this.mockRedis.Object);
        this.invalidateRefreshTokenHandler = new InvalidateRefreshTokenHandler(this.mockRedis.Object);
        this.getUserForRefreshHandler = new GetUserForRefreshHandler(this.context);

        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new Auth.Domains.Token.TokenController(
            this.generateRefreshTokenHandler,
            this.generateTokenStringHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    #region PostAsync Tests

    [Test]
    public void PostAsync_WhenInvalidCredentials_ThrowsPermissionDeniedException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var query = new GenerateTokenQuery("test@example.com", "wrongpassword");

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .Returns(0);

        this.mockGetByEmailHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync((GetByEmailResponse?)null);

        // Act & Assert
        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.controller.PostAsync(
                this.generateTokenHandler,
                query,
                wide,
                cancellationToken));
    }

    [Test]
    public void PostAsync_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var query = new GenerateTokenQuery("test@example.com", "password123");

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .Returns(5);

        // Act & Assert
        Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.controller.PostAsync(
                this.generateTokenHandler,
                query,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task PostAsync_WhenValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var getByEmailResponse = new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password);

        var query = new GenerateTokenQuery("test@example.com", "correctpassword");

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .Returns(0);

        this.mockGetByEmailHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(getByEmailResponse);

        this.mockVerifyPasswordHandler
            .Setup(h => h.Handle(query.Password, user.Password))
            .Returns(true);

        // Act
        var result = await this.controller.PostAsync(
            this.generateTokenHandler,
            query,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var tokenResponse = okResult.Value as TokenResponse;
        Assert.That(tokenResponse, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tokenResponse!.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(tokenResponse.RefreshToken, Is.Not.Null.And.Not.Empty);
            Assert.That(tokenResponse.User.Id, Is.EqualTo(this.testUserId));
            Assert.That(tokenResponse.User.Email, Is.EqualTo("test@example.com"));
            Assert.That(tokenResponse.User.Name, Is.EqualTo("Test User"));
            Assert.That(wide.UserId, Is.EqualTo(query.Email));
        }
    }

    [Test]
    public void PostAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var query = new GenerateTokenQuery("", "password123");

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await this.controller.PostAsync(
                this.generateTokenHandler,
                query,
                wide,
                cancellationToken));
    }

    [Test]
    public async Task PostAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        var getByEmailResponse = new GetByEmailResponse(user.Id, user.Email, user.Name, user.Password);

        var query = new GenerateTokenQuery("test@example.com", "correctpassword");

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .Returns(0);

        this.mockGetByEmailHandler
            .Setup(h => h.Handle(query.Email, cancellationToken))
            .ReturnsAsync(getByEmailResponse);

        this.mockVerifyPasswordHandler
            .Setup(h => h.Handle(query.Password, user.Password))
            .Returns(true);

        // Act
        await this.controller.PostAsync(
            this.generateTokenHandler,
            query,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(query.Email));
    }

    #endregion

    #region Refresh Tests

    [Test]
    public async Task Refresh_WhenInvalidRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var refreshToken = "invalid_refresh_token";
        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Act
        var result = await this.controller.Refresh(
            query,
            this.validateTokenHandler,
            this.invalidateRefreshTokenHandler,
            this.getUserForRefreshHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<BadRequestObjectResult>());

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
    }

    [Test]
    public async Task Refresh_WhenValidRefreshToken_ReturnsOkWithNewToken()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var refreshToken = Guid.NewGuid().ToString();

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Mock Redis to return valid token
        var mockDatabase = new Mock<IDatabase>();
        var refreshTokenResponse =
            new ValidateTokenResponse(refreshToken, DateTime.UtcNow.AddDays(7), this.testUserId, true);
        var serializedToken = System.Text.Json.JsonSerializer.Serialize(refreshTokenResponse);
        mockDatabase
            .Setup(db => db.StringGetAsync(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serializedToken));
        this.mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(mockDatabase.Object);

        // Act
        var result = await this.controller.Refresh(
            query,
            this.validateTokenHandler,
            this.invalidateRefreshTokenHandler,
            this.getUserForRefreshHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));

        var tokenResponse = okResult.Value as TokenResponse;
        Assert.That(tokenResponse, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(tokenResponse!.AccessToken, Is.Not.Null.And.Not.Empty);
            Assert.That(tokenResponse.RefreshToken, Is.Not.Null.And.Not.Empty);
            Assert.That(tokenResponse.User.Id, Is.EqualTo(this.testUserId));
            Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
        }
    }

    [Test]
    public async Task Refresh_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var refreshToken = Guid.NewGuid().ToString();

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = "test@example.com",
            Name = "Test User",
            Password = "hashedPassword"
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Mock Redis to return valid token
        var mockDatabase = new Mock<IDatabase>();
        var refreshTokenResponse = new ValidateTokenResponse(refreshToken, DateTime.UtcNow.AddDays(7), this.testUserId, true);
        var serializedToken = System.Text.Json.JsonSerializer.Serialize(refreshTokenResponse);
        mockDatabase
            .Setup(db => db.StringGetAsync(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serializedToken));
        this.mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(mockDatabase.Object);

        // Act
        await this.controller.Refresh(
            query,
            this.validateTokenHandler,
            this.invalidateRefreshTokenHandler,
            this.getUserForRefreshHandler,
            wide,
            cancellationToken);

        // Assert
        Assert.That(wide.UserId, Is.EqualTo(this.testUserId.ToString()));
    }

    #endregion

    #region Attribute Tests

    [Test]
    public void TokenController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "TokenController should have Authorize attribute");
    }

    [Test]
    public void TokenController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "TokenController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void TokenController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "TokenController should have ApiController attribute");
    }

    [Test]
    public void TokenController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);

        // Act
        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.That(producesAttribute, Is.Not.Null, "TokenController should have Produces attribute");
        Assert.That(producesAttribute!.ContentTypes.FirstOrDefault(), Is.EqualTo("application/json"));
    }

    [Test]
    public void PostAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);
        var methodInfo = controllerType.GetMethod(nameof(Auth.Domains.Token.TokenController.PostAsync));

        // Act
        var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "PostAsync should have AllowAnonymous attribute");
    }

    [Test]
    public void Refresh_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(Auth.Domains.Token.TokenController);
        var methodInfo = controllerType.GetMethod(nameof(Auth.Domains.Token.TokenController.Refresh));

        // Act
        var allowAnonymousAttribute = methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(allowAnonymousAttribute, Is.Not.Null, "Refresh should have AllowAnonymous attribute");
    }

    #endregion
}

using System.Text.Json;

using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;
using Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.ValidateToken;
using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.Handlers;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Auth.Domains.Token.Responses;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.API;
using Fenicia.Common.Data;
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

public class TokenControllerTests : IDisposable
{
    public TokenControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.testUserId = Guid.NewGuid();
        var cache = new MemoryCache(new MemoryCacheOptions());

        this.mockLoginAttemptHandler = new Mock<LoginAttemptService>(cache);
        var mockIncrementAttempts = new Mock<IncrementAttemptsService>(cache);
        this.mockVerifyPasswordHandler = new Mock<VerifyPasswordService>();
        var mockConfiguration = new Mock<IConfiguration>();
        mockConfiguration.Setup(c => c["Jwt:Secret"]).Returns("ThisIsASecretKeyForJwtSigning123456");
        var mockRedis = new Mock<IConnectionMultiplexer>();
        this.mockDatabase = new Mock<IDatabase>();
        mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(this.mockDatabase.Object);

        var generateTokenHandler1 = new GenerateTokenHandler(
            this.context,
            this.mockLoginAttemptHandler.Object,
            mockIncrementAttempts.Object,
            this.mockVerifyPasswordHandler.Object);

        var generateTokenStringHandler1 = new GenerateTokenStringHandler(mockConfiguration.Object);
        var generateRefreshTokenHandler1 = new GenerateRefreshTokenHandler(mockRedis.Object);
        var validateTokenHandler1 = new ValidateTokenHandler(mockRedis.Object);
        var invalidateRefreshTokenHandler1 = new InvalidateRefreshTokenHandler(mockRedis.Object);
        var getUserForRefreshHandler1 = new GetUserForRefreshHandler(this.context);

        var mockHttpContext1 = new Mock<HttpContext>();

        this.controller = new TokenController(
            generateTokenHandler1,
            generateRefreshTokenHandler1,
            generateTokenStringHandler1,
            validateTokenHandler1,
            invalidateRefreshTokenHandler1,
            getUserForRefreshHandler1
            )
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext1.Object
            }
        };

        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly TokenController controller;
    private readonly DefaultContext context;
    private readonly Mock<LoginAttemptService> mockLoginAttemptHandler;
    private readonly Mock<VerifyPasswordService> mockVerifyPasswordHandler;
    private readonly Mock<IDatabase> mockDatabase;
    private readonly Guid testUserId;
    private readonly Faker faker;

    [Fact]
    public async Task PostAsync_WhenInvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var query = new GenerateTokenQuery(this.faker.Internet.Email(), this.faker.Internet.Password());

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email))
            .Returns(0);

        // Act
        var result = await this.controller.PostAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PostAsync_WhenTooManyAttempts_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var query = new GenerateTokenQuery(this.faker.Internet.Email(), this.faker.Internet.Password());

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email))
            .Returns(5);

        // Act
        var result = await this.controller.PostAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PostAsync_WhenValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();
        var hashedPassword = "$2a$12$" + this.faker.Random.String2(53);

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = email,
            Name = name,
            Password = hashedPassword
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GenerateTokenQuery(email, password);

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email))
            .Returns(0);

        this.mockVerifyPasswordHandler
            .Setup(h => h.Handle(query.Password, hashedPassword))
            .Returns(true);

        // Act
        var result = await this.controller.PostAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var tokenResponse = okResult.Value as TokenResponse;
        Assert.NotNull(tokenResponse);
        Assert.NotNull(tokenResponse.AccessToken);
        Assert.NotEmpty(tokenResponse.AccessToken);
        Assert.NotNull(tokenResponse.RefreshToken);
        Assert.NotEmpty(tokenResponse.RefreshToken);
        Assert.Equal(this.testUserId, tokenResponse.User.Id);
        Assert.Equal(email, tokenResponse.User.Email);
        Assert.Equal(name, tokenResponse.User.Name);
        Assert.Equal(query.Email, wide.UserId);
    }

    [Fact]
    public async Task PostAsync_WhenEmailIsNull_ThrowsArgumentException()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var query = new GenerateTokenQuery(string.Empty, this.faker.Internet.Password());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.controller.PostAsync(
                query,
                wide,
                ct));
    }

    [Fact]
    public async Task PostAsync_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();
        var hashedPassword = "$2a$12$" + this.faker.Random.String2(53);

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = email,
            Name = name,
            Password = hashedPassword
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GenerateTokenQuery(email, password);

        this.mockLoginAttemptHandler
            .Setup(h => h.Handle(query.Email))
            .Returns(0);

        this.mockVerifyPasswordHandler
            .Setup(h => h.Handle(query.Password, hashedPassword))
            .Returns(true);

        // Act
        await this.controller.PostAsync(
            query,
            wide,
            ct);

        // Assert
        Assert.Equal(query.Email, wide.UserId);
    }

    [Fact]
    public async Task Refresh_WhenInvalidRefreshToken_ReturnsBadRequest()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        const string refreshToken = "invalid_refresh_token";
        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Act
        var result = await this.controller.Refresh(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<BadRequestObjectResult>(result.Result);

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Refresh_WhenValidRefreshToken_ReturnsOkWithNewToken()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var refreshToken = Guid.NewGuid().ToString();

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Mock Redis to return valid token
        var refreshTokenResponse =
            new ValidateTokenResponse(refreshToken, DateTime.UtcNow.AddDays(7), this.testUserId, true);
        var serializedToken = JsonSerializer.Serialize(refreshTokenResponse);
        this.mockDatabase
            .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serializedToken));

        // Act
        var result = await this.controller.Refresh(
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.Equal(200, okResult.StatusCode);

        var tokenResponse = okResult.Value as TokenResponse;
        Assert.NotNull(tokenResponse);
        Assert.NotNull(tokenResponse.AccessToken);
        Assert.NotEmpty(tokenResponse.AccessToken);
        Assert.NotNull(tokenResponse.RefreshToken);
        Assert.NotEmpty(tokenResponse.RefreshToken);
        Assert.Equal(this.testUserId, tokenResponse.User.Id);
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task Refresh_SetsWideEventContextUserId()
    {
        // Arrange
        var wide = new WideEventContext();
        var ct = CancellationToken.None;

        var refreshToken = Guid.NewGuid().ToString();

        var user = new UserModel
        {
            Id = this.testUserId,
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new ValidateTokenQuery(this.testUserId, refreshToken);

        // Mock Redis to return valid token
        var refreshTokenResponse =
            new ValidateTokenResponse(refreshToken, DateTime.UtcNow.AddDays(7), this.testUserId, true);
        var serializedToken = JsonSerializer.Serialize(refreshTokenResponse);
        this.mockDatabase
            .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(new RedisValue(serializedToken));

        // Act
        await this.controller.Refresh(
            query,
            wide,
            ct);

        // Assert
        Assert.Equal(this.testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void TokenController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void TokenController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void TokenController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void TokenController_HasProducesAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);

        // Act
        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        // Assert
        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
    }

    [Fact]
    public void PostAsync_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);
        var methodInfo = controllerType.GetMethod(nameof(TokenController.PostAsync));

        // Act
        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }

    [Fact]
    public void Refresh_HasAllowAnonymousAttribute()
    {
        // Arrange
        var controllerType = typeof(TokenController);
        var methodInfo = controllerType.GetMethod(nameof(TokenController.Refresh));

        // Act
        var allowAnonymousAttribute =
            methodInfo?.GetCustomAttributes(typeof(AllowAnonymousAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(allowAnonymousAttribute);
    }
}

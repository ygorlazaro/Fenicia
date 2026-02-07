using System.Text.Json;

using Bogus;

using Fenicia.Auth.Domains.RefreshToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private RefreshTokenRepository sut;
    private Mock<IDatabase> redisDbMock;
    private Mock<IConnectionMultiplexer> redisMock;

    [SetUp]
    public void Setup()
    {
        redisDbMock = new Mock<IDatabase>();
        redisMock = new Mock<IConnectionMultiplexer>();
        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDbMock.Object);
        sut = new RefreshTokenRepository(redisMock.Object);
        faker = new Faker();
    }

    [Test]
    public void InvalidateRefreshTokenAsyncHandlesNonExistentToken()
    {
        var nonExistentToken = faker.Random.Hash();

        Assert.DoesNotThrowAsync(() => sut.InvalidateRefreshTokenAsync(nonExistentToken, cancellationToken));
    }

    [Test]
    public void Add_NullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => sut.Add(null!));
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalseForWrongUser()
    {
        var stored = new RefreshToken { Token = "t1", UserId = Guid.NewGuid(), IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        var key = "refresh_token:" + stored.Token;
        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)JsonSerializer.Serialize(stored));

        var result = await sut.ValidateTokenAsync(Guid.NewGuid(), stored.Token, cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalseForExpiredOrInactive()
    {
        var userId = Guid.NewGuid();
        const string token = "tokexp";

        var expired = new RefreshToken { Token = token, UserId = userId, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(-1) };
        var key = "refresh_token:" + token;
        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)JsonSerializer.Serialize(expired));

        var res1 = await sut.ValidateTokenAsync(userId, token, cancellationToken);
        Assert.That(res1, Is.False);

        var inactive = new RefreshToken { Token = token, UserId = userId, IsActive = false, ExpirationDate = DateTime.UtcNow.AddDays(1) };
        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)JsonSerializer.Serialize(inactive));

        var res2 = await sut.ValidateTokenAsync(userId, token, cancellationToken);
        Assert.That(res2, Is.False);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_DoesNothingWhenStoredIsNullJson()
    {
        var token = faker.Random.AlphaNumeric(44);
        var key = "refresh_token:" + token;

        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)"null");

        await sut.InvalidateRefreshTokenAsync(token, cancellationToken);

        Assert.That(redisDbMock.Invocations.Any(i => i.Method.Name is "StringSet" or "StringSetAsync"), Is.False);
    }

    [Test]
    public void InvalidateRefreshTokenAsync_NullArgThrows()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.InvalidateRefreshTokenAsync(null!, cancellationToken));
    }

    [Test]
    public void ValidateTokenAsync_NullArgThrows()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ValidateTokenAsync(Guid.NewGuid(), null!, cancellationToken));
    }

    [Test]
    public async Task ValidateTokenAsyncReturnsFalseForNonExistentToken()
    {
        var userId = Guid.NewGuid();
        var nonExistentToken = faker.Random.Hash();

        var result = await sut.ValidateTokenAsync(userId, nonExistentToken, cancellationToken);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Add_SetsValueInRedis()
    {
        var token = faker.Random.AlphaNumeric(44);
        var refresh = new RefreshToken { Token = token, UserId = Guid.NewGuid(), IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(6) };

        sut.Add(refresh);

        Assert.That(redisDbMock.Invocations.Any(i => i.Method.Name == "StringSet"), Is.True);
    }

    [Test]
    public async Task ValidateTokenAsyncReturnsTrueForValidToken()
    {
        var userId = Guid.NewGuid();
        var token = faker.Random.AlphaNumeric(44);
        var refresh = new RefreshToken { Token = token, UserId = userId, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(2) };

        var key = "refresh_token:" + token;
        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)JsonSerializer.Serialize(refresh));

        var result = await sut.ValidateTokenAsync(userId, token, cancellationToken);

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_UpdatesTokenToInactive()
    {
        var userId = Guid.NewGuid();
        var token = faker.Random.AlphaNumeric(44);
        var refresh = new RefreshToken { Token = token, UserId = userId, IsActive = true, ExpirationDate = DateTime.UtcNow.AddDays(2) };

        var key = "refresh_token:" + token;
        redisDbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)JsonSerializer.Serialize(refresh));

        await sut.InvalidateRefreshTokenAsync(token, cancellationToken);

        var invocation = redisDbMock.Invocations.FirstOrDefault(i => i.Method.Name == "StringSetAsync");
        Assert.That(invocation, Is.Not.Null, "Expected StringSetAsync to be called.");

        var setValue = invocation!.Arguments.ElementAtOrDefault(1) as RedisValue?;
        Assert.That(setValue, Is.Not.Null, "Expected StringSetAsync to be called with a value argument.");

        var updated = JsonSerializer.Deserialize<RefreshToken>(setValue!.ToString()!);

        Assert.That(updated, Is.Not.Null);
        Assert.That(updated!.IsActive, Is.False);
    }
}

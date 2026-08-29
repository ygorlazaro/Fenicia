using Fenicia.Auth.Domains.RefreshToken;
using Moq;
using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class RefreshTokenRepositoryTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly RefreshTokenRepository _repository;

    public RefreshTokenRepositoryTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();

        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_redisDbMock.Object);
        _repository = new RefreshTokenRepository(_redisMock.Object);
    }

    [Fact]
    public async Task AddAsync_WhenTokenIsValid_StoresInRedis()
    {
        var token = new RefreshTokenModel("test_token", DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        await _repository.AddAsync(token);

        _redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == "refresh_token:test_token"),
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenTokenExists_ReturnsToken()
    {
        const string token = "existing_token";
        var tokenModel = new RefreshTokenModel(token, DateTime.UtcNow.AddDays(7), Guid.NewGuid());
        var redisValue = new RedisValue(System.Text.Json.JsonSerializer.Serialize(tokenModel));

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == $"refresh_token:{token}"), CommandFlags.None)).ReturnsAsync(redisValue);

        var result = await _repository.GetAsync(token);

        Assert.NotNull(result);
        Assert.Equal(token, result.Token);
    }

    [Fact]
    public async Task GetAsync_WhenTokenDoesNotExist_ReturnsNull()
    {
        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None)).ReturnsAsync(RedisValue.Null);

        var result = await _repository.GetAsync("non_existent_token");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_WhenTokenIsEmpty_ReturnsNull()
    {
        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None)).ReturnsAsync(RedisValue.Null);

        var result = await _repository.GetAsync(string.Empty);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WhenTokenExists_UpdatesInRedis()
    {
        const string token = "token_to_update";
        var tokenModel = new RefreshTokenModel(token, DateTime.UtcNow.AddDays(7), Guid.NewGuid()) { IsActive = true };
        var updatedTokenModel = tokenModel with { IsActive = false };

        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None)).ReturnsAsync(RedisValue.Null);

        await _repository.UpdateAsync(updatedTokenModel);

        _redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == $"refresh_token:{token}"),
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                CommandFlags.None),
            Times.Once);
    }

    public void Dispose()
    {
    }
}

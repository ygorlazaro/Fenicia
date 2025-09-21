namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using StackExchange.Redis;
using Fenicia.Auth.Domains.RefreshToken;
using Moq;

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
        this.redisDbMock = new Mock<IDatabase>();
        this.redisMock = new Mock<IConnectionMultiplexer>();
        this.redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(this.redisDbMock.Object);
        this.sut = new RefreshTokenRepository(this.redisMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public void AddSavesRefreshTokenToRedis()
    {
        // Arrange
        var refreshToken = new RefreshToken
        {
            Token = this.faker.Random.Hash(),
            UserId = Guid.NewGuid(),
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };

        this.redisDbMock.Setup(x => x.StringSet(
            It.Is<RedisKey>(k => k.ToString().Contains(refreshToken.Token)),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>())).Returns(true);

        // Act
        this.sut.Add(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSet(
            It.Is<RedisKey>(k => k.ToString().Contains(refreshToken.Token)),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<bool>(),
            It.IsAny<When>(),
            It.IsAny<CommandFlags>()), Times.Once);
    }

    [Test]
    public void InvalidateRefreshTokenAsyncHandlesNonExistentToken()
    {
        // Arrange
        var nonExistentToken = this.faker.Random.Hash();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => this.sut.InvalidateRefreshTokenAsync(nonExistentToken, this.cancellationToken));
    }

    [Test]
    public async Task ValidateTokenAsyncReturnsFalseForNonExistentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentToken = this.faker.Random.Hash();

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, nonExistentToken, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }
}

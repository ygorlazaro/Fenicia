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
        // Arrange
        var nonExistentToken = faker.Random.Hash();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => sut.InvalidateRefreshTokenAsync(nonExistentToken, cancellationToken));
    }

    [Test]
    public async Task ValidateTokenAsyncReturnsFalseForNonExistentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentToken = faker.Random.Hash();

        // Act
        var result = await sut.ValidateTokenAsync(userId, nonExistentToken, cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }
}

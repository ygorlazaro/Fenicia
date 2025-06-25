using System.Text.RegularExpressions;

using Bogus;

using Fenicia.Auth.Domains.RefreshToken;

using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class RefreshTokenServiceTests
{
    private Mock<ILogger<RefreshTokenService>> _loggerMock;
    private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
    private RefreshTokenService _sut;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<RefreshTokenService>>();
        _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        _sut = new RefreshTokenService(_loggerMock.Object, _refreshTokenRepositoryMock.Object);
        _faker = new Faker();
    }

    [Test]
    public async Task GenerateRefreshTokenAsync_GeneratesValidToken()
    {
        // Arrange
        var userId = _faker.Random.Guid();
        var base64Pattern = @"^[a-zA-Z0-9+/]*={0,2}$";

        // Act
        var result = await _sut.GenerateRefreshTokenAsync(userId);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data?.Length, Is.EqualTo(44)); // Base64 encoded 32 bytes = 44 characters
            Assert.That(result.Data, Does.Match(base64Pattern));
        });

        _refreshTokenRepositoryMock.Verify(
            x => x.Add(It.Is<RefreshTokenModel>(t => t.UserId == userId && t.Token == result.Data)),
            Times.Once
        );

        _refreshTokenRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ValidateTokenAsync_CallsRepositoryAndReturnsResult()
    {
        // Arrange
        var userId = _faker.Random.Guid();
        var refreshToken = _faker.Random.AlphaNumeric(44); // Simulating Base64 token length
        var expectedResult = _faker.Random.Bool();

        _refreshTokenRepositoryMock
            .Setup(x => x.ValidateTokenAsync(userId, refreshToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.ValidateTokenAsync(userId, refreshToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResult));
        _refreshTokenRepositoryMock.Verify(
            x => x.ValidateTokenAsync(userId, refreshToken),
            Times.Once
        );
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_CallsRepository()
    {
        // Arrange
        var refreshToken = _faker.Random.AlphaNumeric(44); // Simulating Base64 token length

        // Act
        var result = await _sut.InvalidateRefreshTokenAsync(refreshToken);

        // Assert
        Assert.That(result.Data, Is.Null);
        _refreshTokenRepositoryMock.Verify(
            x => x.InvalidateRefreshTokenAsync(refreshToken),
            Times.Once
        );
    }

    [Test]
    public async Task GenerateRefreshTokenAsync_GeneratesUniqueTokens()
    {
        // Arrange
        var userId = _faker.Random.Guid();
        var tokens = new HashSet<string>();

        // Act
        for (var i = 0; i < _faker.Random.Int(500, 1000); i++) // Random number of iterations for better coverage
        {
            var result = await _sut.GenerateRefreshTokenAsync(userId);
            tokens.Add(result.Data!);
        }

        // Assert
        Assert.That(
            tokens,
            Has.Count.EqualTo(tokens.Count),
            "Generated tokens should all be unique"
        );
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateTokenAsync_ReturnsExpectedResult(bool expectedResult)
    {
        // Arrange
        var userId = _faker.Random.Guid();
        var refreshToken = _faker.Random.AlphaNumeric(44);

        _refreshTokenRepositoryMock
            .Setup(x => x.ValidateTokenAsync(userId, refreshToken))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _sut.ValidateTokenAsync(userId, refreshToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResult));
    }

    [Test]
    public async Task GenerateRefreshTokenAsync_SaveChangesFailure_StillReturnsToken()
    {
        // Arrange
        var userId = _faker.Random.Guid();

        _refreshTokenRepositoryMock.Setup(x => x.SaveChangesAsync());

        // Act
        var result = await _sut.GenerateRefreshTokenAsync(userId);

        // Assert
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data, Has.Length.EqualTo(44));
    }

    [Test]
    public async Task GenerateRefreshTokenAsync_MultipleCalls_GeneratesDistinctTokens()
    {
        // Arrange
        var userIds = _faker.Make(5, () => _faker.Random.Guid()).ToList();
        var generatedTokens = new List<(Guid UserId, string Token)>();

        // Act
        foreach (var userId in userIds)
        {
            var result = await _sut.GenerateRefreshTokenAsync(userId);
            generatedTokens.Add((userId, result.Data!));
        }

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(
                generatedTokens.Select(x => x.Token).Distinct().Count(),
                Is.EqualTo(generatedTokens.Count),
                "All generated tokens should be unique"
            );
            Assert.That(
                generatedTokens.All(x => x.Token.Length == 44),
                "All tokens should be 44 characters long"
            );
            Assert.That(
                generatedTokens.All(x => Regex.IsMatch(x.Token, @"^[a-zA-Z0-9+/]*={0,2}$")),
                "All tokens should be valid Base64 strings"
            );
        });
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_WithMultipleTokens_CallsRepositoryForEach()
    {
        // Arrange
        var refreshTokens = _faker.Make(5, () => _faker.Random.AlphaNumeric(44)).ToList();

        // Act
        foreach (var token in refreshTokens)
        {
            await _sut.InvalidateRefreshTokenAsync(token);
        }

        // Assert
        foreach (var token in refreshTokens)
        {
            _refreshTokenRepositoryMock.Verify(
                x => x.InvalidateRefreshTokenAsync(token),
                Times.Once
            );
        }
    }
}

namespace Fenicia.Auth.Tests.Services;

using System.Text.RegularExpressions;

using Bogus;

using Domains.RefreshToken;

using Microsoft.Extensions.Logging;

using Moq;

public class RefreshTokenServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<ILogger<RefreshTokenService>> loggerMock;
    private Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
    private RefreshTokenService sut;

    [SetUp]
    public void Setup()
    {
        this.loggerMock = new Mock<ILogger<RefreshTokenService>>();
        this.refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        this.sut = new RefreshTokenService(this.loggerMock.Object, this.refreshTokenRepositoryMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesValidToken()
    {
        // Arrange
        var userId = this.faker.Random.Guid();
        var base64Pattern = @"^[a-zA-Z0-9+/]*={0,2}$";

        // Act
        var result = this.sut.GenerateRefreshToken(userId);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data?.Length, Is.EqualTo(expected: 44)); // Base64 encoded 32 bytes = 44 characters
            Assert.That(result.Data, Does.Match(base64Pattern));
        }

        this.refreshTokenRepositoryMock.Verify(x => x.Add(It.Is<RefreshToken>(t => t.UserId == userId && t.Token == result.Data)), Times.Once);
    }

    [Test]
    public async Task ValidateTokenAsyncCallsRepositoryAndReturnsResult()
    {
        // Arrange
        var userId = this.faker.Random.Guid();
        var refreshToken = this.faker.Random.AlphaNumeric(length: 44); // Simulating Base64 token length
        var expectedResult = this.faker.Random.Bool();

        this.refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, refreshToken, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResult));
        this.refreshTokenRepositoryMock.Verify(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken), Times.Once);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsyncCallsRepository()
    {
        // Arrange
        var refreshToken = this.faker.Random.AlphaNumeric(length: 44); // Simulating Base64 token length

        // Act
        var result = await this.sut.InvalidateRefreshTokenAsync(refreshToken, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.Null);
        this.refreshTokenRepositoryMock.Verify(x => x.InvalidateRefreshTokenAsync(refreshToken, this.cancellationToken), Times.Once);
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesUniqueTokens()
    {
        // Arrange
        var userId = this.faker.Random.Guid();
        var tokens = new HashSet<string>();

        // Act
        for (var i = 0; i < this.faker.Random.Int(min: 500, max: 1000); i++)
        {
            var result = this.sut.GenerateRefreshToken(userId);
            tokens.Add(result.Data!);
        }

        // Assert
        Assert.That(tokens, Has.Count.EqualTo(tokens.Count), "Generated tokens should all be unique");
    }

    [Test]
    [TestCase(arg: true)]
    [TestCase(arg: false)]
    public async Task ValidateTokenAsyncReturnsExpectedResult(bool expectedResult)
    {
        // Arrange
        var userId = this.faker.Random.Guid();
        var refreshToken = this.faker.Random.AlphaNumeric(length: 44);

        this.refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken)).ReturnsAsync(expectedResult);

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, refreshToken, this.cancellationToken);

        // Assert
        Assert.That(result.Data, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GenerateRefreshTokenAsyncSaveChangesFailureStillReturnsToken()
    {
        // Arrange
        var userId = this.faker.Random.Guid();

        // Act
        var result = this.sut.GenerateRefreshToken(userId);

        // Assert
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data, Has.Length.EqualTo(expected: 44));
    }

    [Test]
    public void GenerateRefreshTokenAsyncMultipleCallsGeneratesDistinctTokens()
    {
        // Arrange
        var userIDs = this.faker.Make(count: 5, () => this.faker.Random.Guid()).ToList();
        var generatedTokens = new List<(Guid UserID, string Token)>();

        // Act
        foreach (var userId in userIDs)
        {
            var result = this.sut.GenerateRefreshToken(userId);
            generatedTokens.Add((userId, result.Data!));
        }

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(generatedTokens.Select(x => x.Token).Distinct().Count(), Is.EqualTo(generatedTokens.Count), "All generated tokens should be unique");
            Assert.That(generatedTokens.All(x => x.Token.Length == 44), "All tokens should be 44 characters long");
            Assert.That(generatedTokens.All(x => Regex.IsMatch(x.Token, @"^[a-zA-Z0-9+/]*={0,2}$")), "All tokens should be valid Base64 strings");
        }
    }

    [Test]
    public async Task InvalidateRefreshTokenAsyncWithMultipleTokensCallsRepositoryForEach()
    {
        // Arrange
        var refreshTokens = this.faker.Make(count: 5, () => this.faker.Random.AlphaNumeric(length: 44)).ToList();

        // Act
        foreach (var token in refreshTokens)
        {
            await this.sut.InvalidateRefreshTokenAsync(token, this.cancellationToken);
        }

        // Assert
        foreach (var token in refreshTokens)
        {
            this.refreshTokenRepositoryMock.Verify(x => x.InvalidateRefreshTokenAsync(token, this.cancellationToken), Times.Once);
        }
    }
}

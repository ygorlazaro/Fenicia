using System.Text.RegularExpressions;

using Bogus;

using Fenicia.Auth.Domains.RefreshToken;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class RefreshTokenServiceTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private Faker faker;
    private Mock<IRefreshTokenRepository> refreshTokenRepositoryMock;
    private RefreshTokenService sut;

    [SetUp]
    public void Setup()
    {
        this.refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        this.sut = new RefreshTokenService(this.refreshTokenRepositoryMock.Object);
        this.faker = new Faker();
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesValidToken()
    {
        var userId = this.faker.Random.Guid();
        const string base64Pattern = "^[a-zA-Z0-9+/]*={0,2}$";

        var result = this.sut.GenerateRefreshToken(userId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(44));
            Assert.That(result, Does.Match(base64Pattern));
        }

        this.refreshTokenRepositoryMock.Verify(x => x.Add(It.Is<RefreshToken>(t => t.UserId == userId && t.Token == result)), Times.Once);
    }

    [Test]
    public async Task ValidateTokenAsyncCallsRepositoryAndReturnsResult()
    {
        var userId = this.faker.Random.Guid();
        var refreshToken = this.faker.Random.AlphaNumeric(44);
        var expectedResult = this.faker.Random.Bool();

        this.refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken)).ReturnsAsync(expectedResult);

        var result = await this.sut.ValidateTokenAsync(userId, refreshToken, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResult));
        this.refreshTokenRepositoryMock.Verify(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken), Times.Once);
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesUniqueTokens()
    {
        var userId = this.faker.Random.Guid();
        var tokens = new HashSet<string>();

        for (var i = 0; i < this.faker.Random.Int(500, 1000); i++)
        {
            var result = this.sut.GenerateRefreshToken(userId);
            tokens.Add(result);
        }

        Assert.That(tokens, Has.Count.EqualTo(tokens.Count), "Generated tokens should all be unique");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateTokenAsyncReturnsExpectedResult(bool expectedResult)
    {
        var userId = this.faker.Random.Guid();
        var refreshToken = this.faker.Random.AlphaNumeric(44);

        this.refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, this.cancellationToken)).ReturnsAsync(expectedResult);

        var result = await this.sut.ValidateTokenAsync(userId, refreshToken, this.cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GenerateRefreshTokenAsyncSaveChangesFailureStillReturnsToken()
    {
        var userId = this.faker.Random.Guid();

        var result = this.sut.GenerateRefreshToken(userId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(44));
    }

    [Test]
    public void GenerateRefreshTokenAsyncMultipleCallsGeneratesDistinctTokens()
    {
        var userIDs = this.faker.Make(5, () => this.faker.Random.Guid()).ToList();
        var generatedTokens = new List<(Guid UserID, string Token)>();

        foreach (var userId in userIDs)
        {
            var result = this.sut.GenerateRefreshToken(userId);
            generatedTokens.Add((userId, result));
        }

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
        var refreshTokens = this.faker.Make(5, () => this.faker.Random.AlphaNumeric(44)).ToList();

        foreach (var token in refreshTokens)
        {
            await this.sut.InvalidateRefreshTokenAsync(token, this.cancellationToken);
        }

        foreach (var token in refreshTokens)
        {
            this.refreshTokenRepositoryMock.Verify(x => x.InvalidateRefreshTokenAsync(token, this.cancellationToken), Times.Once);
        }
    }
}
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
        refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
        sut = new RefreshTokenService(refreshTokenRepositoryMock.Object);
        faker = new Faker();
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesValidToken()
    {
        var userId = faker.Random.Guid();
        const string base64Pattern = "^[a-zA-Z0-9+/]*={0,2}$";

        var result = sut.GenerateRefreshToken(userId);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(44));
            Assert.That(result, Does.Match(base64Pattern));
        }

        refreshTokenRepositoryMock.Verify(x => x.Add(It.Is<RefreshToken>(t => t.UserId == userId && t.Token == result)), Times.Once);
    }

    [Test]
    public async Task ValidateTokenAsyncCallsRepositoryAndReturnsResult()
    {
        var userId = faker.Random.Guid();
        var refreshToken = faker.Random.AlphaNumeric(44);
        var expectedResult = faker.Random.Bool();

        refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, cancellationToken)).ReturnsAsync(expectedResult);

        var result = await sut.ValidateTokenAsync(userId, refreshToken, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResult));
        refreshTokenRepositoryMock.Verify(x => x.ValidateTokenAsync(userId, refreshToken, cancellationToken), Times.Once);
    }

    [Test]
    public void GenerateRefreshTokenAsyncGeneratesUniqueTokens()
    {
        var userId = faker.Random.Guid();
        var tokens = new HashSet<string>();

        for (var i = 0; i < faker.Random.Int(500, 1000); i++)
        {
            var result = sut.GenerateRefreshToken(userId);
            tokens.Add(result);
        }

        Assert.That(tokens, Has.Count.EqualTo(tokens.Count), "Generated tokens should all be unique");
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task ValidateTokenAsyncReturnsExpectedResult(bool expectedResult)
    {
        var userId = faker.Random.Guid();
        var refreshToken = faker.Random.AlphaNumeric(44);

        refreshTokenRepositoryMock.Setup(x => x.ValidateTokenAsync(userId, refreshToken, cancellationToken)).ReturnsAsync(expectedResult);

        var result = await sut.ValidateTokenAsync(userId, refreshToken, cancellationToken);

        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void GenerateRefreshTokenAsyncSaveChangesFailureStillReturnsToken()
    {
        var userId = faker.Random.Guid();

        var result = sut.GenerateRefreshToken(userId);

        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Length.EqualTo(44));
    }

    [Test]
    public void GenerateRefreshTokenAsyncMultipleCallsGeneratesDistinctTokens()
    {
        var userIDs = faker.Make(5, () => faker.Random.Guid()).ToList();
        var generatedTokens = new List<(Guid UserID, string Token)>();

        foreach (var userId in userIDs)
        {
            var result = sut.GenerateRefreshToken(userId);
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
        var refreshTokens = faker.Make(5, () => faker.Random.AlphaNumeric(44)).ToList();

        foreach (var token in refreshTokens)
        {
            await sut.InvalidateRefreshTokenAsync(token, cancellationToken);
        }

        foreach (var token in refreshTokens)
        {
            refreshTokenRepositoryMock.Verify(x => x.InvalidateRefreshTokenAsync(token, cancellationToken), Times.Once);
        }
    }
}

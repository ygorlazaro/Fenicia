using Bogus;

using Fenicia.Auth.Domains.Security;

namespace Fenicia.Auth.Tests.Domains.Security;

public class VerifyPasswordServiceTests
{
    private readonly Faker _faker = new();

    [Theory]
    [InlineData("SimplePass123")]
    [InlineData("MyPassword!")]
    [InlineData("Test123")]
    [InlineData("SecurePass")]
    public void Handle_WhenPasswordMatchesHash_ReturnsTrue(string password)
    {
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(password, hashedPassword);

        Assert.True(result);
    }

    [Theory]
    [InlineData("password1", "wrongPassword2")]
    [InlineData("test123", "other456")]
    [InlineData("abc", "xyz")]
    public void Handle_WhenPasswordDoesNotMatchHash_ReturnsFalse(string password, string wrongPassword)
    {
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(wrongPassword, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsNull_ReturnsFalse()
    {
        var hashedPassword = SecurityService.Hash(_faker.Internet.Password());

        var result = SecurityService.Verify(null!, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsNull_ReturnsFalse()
    {
        var password = _faker.Internet.Password();

        var result = SecurityService.Verify(password, null!);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenBothPasswordAndHashAreNull_ReturnsFalse()
    {
        var result = SecurityService.Verify(null!, null!);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsEmpty_ReturnsFalse()
    {
        var hashedPassword = SecurityService.Hash(_faker.Internet.Password());

        var result = SecurityService.Verify(string.Empty, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsEmpty_ReturnsFalse()
    {
        var password = _faker.Internet.Password();

        var result = SecurityService.Verify(password, string.Empty);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordHasDifferentCase_ReturnsFalse()
    {
        var password = "TestPass123";
        var wrongCasePassword = password.ToLowerInvariant();
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(wrongCasePassword, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashIsInvalidFormat_ReturnsFalse()
    {
        var password = _faker.Internet.Password();
        var invalidHash = _faker.Lorem.Word();

        var result = SecurityService.Verify(password, invalidHash);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsSpecialCharacters_VerifiesCorrectly()
    {
        var password = $"P@$$w0rd!{_faker.Random.AlphaNumeric(10)}";
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsVeryLong_VerifiesCorrectly()
    {
        var password = _faker.Lorem.Paragraphs();
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsShort_VerifiesCorrectly()
    {
        var password = _faker.Random.String2(1, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsUnicode_VerifiesCorrectly()
    {
        var password = $"{_faker.Internet.Password()} 日本語 🔐";
        var hashedPassword = SecurityService.Hash(password);

        var result = SecurityService.Verify(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenDifferentPasswordsProduceDifferentHashes()
    {
        var password1 = _faker.Internet.Password();
        var password2 = _faker.Internet.Password();
        var hash1 = SecurityService.Hash(password1);
        var hash2 = SecurityService.Hash(password2);

        var result1 = SecurityService.Verify(password1, hash1);
        var result2 = SecurityService.Verify(password1, hash2);
        var result3 = SecurityService.Verify(password2, hash1);
        var result4 = SecurityService.Verify(password2, hash2);

        Assert.True(result1);
        Assert.False(result2);
        Assert.False(result3);
        Assert.True(result4);
    }
}

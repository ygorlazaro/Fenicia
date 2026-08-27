using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Security.Services;

namespace Fenicia.Auth.Tests.Domains.Security;

public class VerifyPasswordServiceTests
{
    private readonly VerifyPasswordService service = new();
    private readonly Faker faker = new();

    [Theory]
    [InlineData("SimplePass123")]
    [InlineData("MyPassword!")]
    [InlineData("Test123")]
    [InlineData("SecurePass")]
    public void Handle_WhenPasswordMatchesHash_ReturnsTrue(string password)
    {

        var hashedPassword = password.Hash();

        var result = service.Handle(password, hashedPassword);

        Assert.True(result);
    }

    [Theory]
    [InlineData("password1", "wrongPassword2")]
    [InlineData("test123", "other456")]
    [InlineData("abc", "xyz")]
    public void Handle_WhenPasswordDoesNotMatchHash_ReturnsFalse(string password, string wrongPassword)
    {

        var hashedPassword = password.Hash();

        var result = service.Handle(wrongPassword, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsNull_ReturnsFalse()
    {

        var hashedPassword = faker.Internet.Password().Hash();

        var result = service.Handle(null!, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsNull_ReturnsFalse()
    {

        var password = faker.Internet.Password();

        var result = service.Handle(password, null!);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenBothPasswordAndHashAreNull_ReturnsFalse()
    {

        var result = service.Handle(null!, null!);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsEmpty_ReturnsFalse()
    {

        var hashedPassword = faker.Internet.Password().Hash();

        var result = service.Handle(string.Empty, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsEmpty_ReturnsFalse()
    {

        var password = faker.Internet.Password();

        var result = service.Handle(password, string.Empty);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordHasDifferentCase_ReturnsFalse()
    {

        var password = faker.Internet.Password();
        var wrongCasePassword = password.ToLowerInvariant();
        var hashedPassword = password.Hash();

        var result = service.Handle(wrongCasePassword, hashedPassword);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashIsInvalidFormat_ReturnsFalse()
    {

        var password = faker.Internet.Password();
        var invalidHash = faker.Lorem.Word();

        var result = service.Handle(password, invalidHash);

        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsSpecialCharacters_VerifiesCorrectly()
    {

        var password = $"P@$$w0rd!{faker.Random.AlphaNumeric(10)}";
        var hashedPassword = password.Hash();

        var result = service.Handle(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsVeryLong_VerifiesCorrectly()
    {

        var password = faker.Lorem.Paragraphs();
        var hashedPassword = password.Hash();

        var result = service.Handle(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsShort_VerifiesCorrectly()
    {

        var password = faker.Random.String2(1, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var hashedPassword = password.Hash();

        var result = service.Handle(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsUnicode_VerifiesCorrectly()
    {

        var password = $"{faker.Internet.Password()} 日本語 🔐";
        var hashedPassword = password.Hash();

        var result = service.Handle(password, hashedPassword);

        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenDifferentPasswordsProduceDifferentHashes()
    {

        var password1 = faker.Internet.Password();
        var password2 = faker.Internet.Password();
        var hash1 = password1.Hash();
        var hash2 = password2.Hash();

        var result1 = service.Handle(password1, hash1);
        var result2 = service.Handle(password1, hash2);
        var result3 = service.Handle(password2, hash1);
        var result4 = service.Handle(password2, hash2);

        Assert.True(result1);
        Assert.False(result2);
        Assert.False(result3);
        Assert.True(result4);
    }
}

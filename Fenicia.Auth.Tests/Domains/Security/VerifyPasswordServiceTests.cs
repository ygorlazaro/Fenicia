using Bogus;

using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Security.Services;

namespace Fenicia.Auth.Tests.Domains.Security;

public class VerifyPasswordServiceTests
{
    private readonly VerifyPasswordService _service = new();
    private readonly Faker faker = new();

    [Theory]
    [InlineData("SimplePass123")]
    [InlineData("MyPassword!")]
    [InlineData("Test123")]
    [InlineData("SecurePass")]
    public void Handle_WhenPasswordMatchesHash_ReturnsTrue(string password)
    {
        // Arrange
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("password1", "wrongPassword2")]
    [InlineData("test123", "other456")]
    [InlineData("abc", "xyz")]
    public void Handle_WhenPasswordDoesNotMatchHash_ReturnsFalse(string password, string wrongPassword)
    {
        // Arrange
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(wrongPassword, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.faker.Internet.Password().Hash();

        // Act
        var result = this._service.Handle(null!, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this._service.Handle(password, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenBothPasswordAndHashAreNull_ReturnsFalse()
    {
        // Act
        var result = this._service.Handle(null!, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.faker.Internet.Password().Hash();

        // Act
        var result = this._service.Handle(string.Empty, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this._service.Handle(password, string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var wrongCasePassword = password.ToLowerInvariant();
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(wrongCasePassword, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashIsInvalidFormat_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var invalidHash = this.faker.Lorem.Word();

        // Act
        var result = this._service.Handle(password, invalidHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsSpecialCharacters_VerifiesCorrectly()
    {
        // Arrange
        var password = $"P@$$w0rd!{this.faker.Random.AlphaNumeric(10)}";
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsVeryLong_VerifiesCorrectly()
    {
        // Arrange
        var password = this.faker.Lorem.Paragraphs();
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsShort_VerifiesCorrectly()
    {
        // Arrange
        // Use ASCII range to avoid Unicode surrogate characters that cause encoding issues in BCrypt
        var password = this.faker.Random.String2(1, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsUnicode_VerifiesCorrectly()
    {
        // Arrange
        var password = $"{this.faker.Internet.Password()} 日本語 🔐";
        var hashedPassword = password.Hash();

        // Act
        var result = this._service.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenDifferentPasswordsProduceDifferentHashes()
    {
        // Arrange
        var password1 = this.faker.Internet.Password();
        var password2 = this.faker.Internet.Password();
        var hash1 = password1.Hash();
        var hash2 = password2.Hash();

        // Act
        var result1 = this._service.Handle(password1, hash1);
        var result2 = this._service.Handle(password1, hash2);
        var result3 = this._service.Handle(password2, hash1);
        var result4 = this._service.Handle(password2, hash2);

        // Assert
        Assert.True(result1);
        Assert.False(result2);
        Assert.False(result3);
        Assert.True(result4);
    }
}
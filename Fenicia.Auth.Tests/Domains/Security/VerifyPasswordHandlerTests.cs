using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.Security.VerifyPassword;

namespace Fenicia.Auth.Tests.Domains.Security;

public class VerifyPasswordHandlerTests
{
    private readonly Faker faker = new();
    private readonly VerifyPasswordHandler handler = new();
    private readonly HashPasswordHandler hashPasswordHandler = new();

    [Theory]
    [InlineData("SimplePass123")]
    [InlineData("MyPassword!")]
    [InlineData("Test123")]
    [InlineData("SecurePass")]
    public void Handle_WhenPasswordMatchesHash_ReturnsTrue(string password)
    {
        // Arrange
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

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
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(wrongPassword, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.hashPasswordHandler.Handle(this.faker.Internet.Password());

        // Act
        var result = this.handler.Handle(null!, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this.handler.Handle(password, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenBothPasswordAndHashAreNull_ReturnsFalse()
    {
        // Act
        var result = this.handler.Handle(null!, null!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.hashPasswordHandler.Handle(this.faker.Internet.Password());

        // Act
        var result = this.handler.Handle(string.Empty, hashedPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenHashedPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this.handler.Handle(password, string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var wrongCasePassword = password.ToLowerInvariant();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(wrongCasePassword, hashedPassword);

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
        var result = this.handler.Handle(password, invalidHash);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsSpecialCharacters_VerifiesCorrectly()
    {
        // Arrange
        var password = $"P@$$w0rd!{this.faker.Random.AlphaNumeric(10)}";
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsVeryLong_VerifiesCorrectly()
    {
        // Arrange
        var password = this.faker.Lorem.Paragraphs();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordIsShort_VerifiesCorrectly()
    {
        // Arrange
        // Use ASCII range to avoid Unicode surrogate characters that cause encoding issues in BCrypt
        var password = this.faker.Random.String2(1, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenPasswordContainsUnicode_VerifiesCorrectly()
    {
        // Arrange
        var password = $"{this.faker.Internet.Password()} 日本語 🔐";
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Handle_WhenDifferentPasswordsProduceDifferentHashes()
    {
        // Arrange
        var password1 = this.faker.Internet.Password();
        var password2 = this.faker.Internet.Password();
        var hash1 = this.hashPasswordHandler.Handle(password1);
        var hash2 = this.hashPasswordHandler.Handle(password2);

        // Act
        var result1 = this.handler.Handle(password1, hash1);
        var result2 = this.handler.Handle(password1, hash2);
        var result3 = this.handler.Handle(password2, hash1);
        var result4 = this.handler.Handle(password2, hash2);

        // Assert
        Assert.True(result1);
        Assert.False(result2);
        Assert.False(result3);
        Assert.True(result4);
    }
}

using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Tests.Domains.Security;

public class HashPasswordHandlerTests
{
    private readonly Faker faker = new();
    private readonly HashPasswordHandler handler = new();

    [Theory]
    [InlineData("SimplePass123")]
    [InlineData("P@$$w0rd!")]
    [InlineData("MySecurePassword")]
    [InlineData("Test123!")]
    [InlineData("Complex!Password#2024")]
    public void Handle_WhenValidPassword_ReturnsHashedPassword(string password)
    {
        // Arrange

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
        Assert.True(result.Length > password.Length);
    }

    [Fact]
    public void Handle_WhenSamePasswordIsHashedTwice_ReturnsDifferentHashes()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var hash1 = this.handler.Handle(password);
        var hash2 = this.handler.Handle(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var password = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<InvalidRequestException>(() => this.handler.Handle(password));
        Assert.Equal("Password cannot be null or empty", ex.Message);
    }

    [Fact]
    public void Handle_WhenPasswordIsNull_ThrowsArgumentException()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        var ex = Assert.Throws<InvalidRequestException>(() => this.handler.Handle(password!));
        Assert.Equal("Password cannot be null or empty", ex.Message);
    }

    [Theory]
    [InlineData("P@$$w0rd!")]
    [InlineData("Test#123!")]
    [InlineData("Secure&Pass")]
    public void Handle_WhenPasswordContainsSpecialCharacters_ReturnsHashedPassword(string password)
    {
        // Arrange

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void Handle_WhenPasswordIsVeryLong_ReturnsHashedPassword()
    {
        // Arrange
        var password = this.faker.Lorem.Paragraphs(5);

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("X")]
    public void Handle_WhenPasswordIsShort_ReturnsHashedPassword(string password)
    {
        // Arrange

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }

    [Theory]
    [InlineData("Password 日本語 🔐")]
    [InlineData("Test ñ 123")]
    [InlineData("Hello 🌍 World")]
    public void Handle_WhenPasswordContainsUnicode_ReturnsHashedPassword(string password)
    {
        // Arrange

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }

    [Fact]
    public void Handle_VerifiedPasswordCanBeVerified()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var hashedPassword = this.handler.Handle(password);

        // Assert
        var verifyHandler = new VerifyPasswordService();
        var isValid = verifyHandler.Handle(password, hashedPassword);
        Assert.True(isValid);
    }

    [Theory]
    [InlineData("  password123  ")]
    [InlineData(" test with spaces ")]
    [InlineData("  multiple  spaces  ")]
    public void Handle_WhenPasswordHasWhitespace_ReturnsHashedPassword(string password)
    {
        // Arrange

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(password, result);
    }
}

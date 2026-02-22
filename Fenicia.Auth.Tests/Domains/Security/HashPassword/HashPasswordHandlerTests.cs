using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Tests.Domains.Security.HashPassword;

[TestFixture]
public class HashPasswordHandlerTests
{
    private HashPasswordHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        this.handler = new HashPasswordHandler();
    }

    [Test]
    public void Handle_WhenValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
        Assert.That(result.Length, Is.GreaterThan(password.Length), "Hashed password should be longer");
    }

    [Test]
    public void Handle_WhenSamePasswordIsHashedTwice_ReturnsDifferentHashes()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hash1 = this.handler.Handle(password);
        var hash2 = this.handler.Handle(password);

        // Assert
        Assert.That(hash1, Is.Not.EqualTo(hash2), "Same password should produce different hashes (due to salt)");
    }

    [Test]
    public void Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var password = string.Empty;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => this.handler.Handle(password));
        Assert.That(ex?.Message, Is.EqualTo("Invalid password"));
    }

    [Test]
    public void Handle_WhenPasswordIsNull_ThrowsArgumentException()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => this.handler.Handle(password!));
        Assert.That(ex?.Message, Is.EqualTo("Invalid password"));
    }

    [Test]
    public void Handle_WhenPasswordContainsSpecialCharacters_ReturnsHashedPassword()
    {
        // Arrange
        var password = "P@$$w0rd!#$%^&*()_+";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }

    [Test]
    public void Handle_WhenPasswordIsVeryLong_ReturnsHashedPassword()
    {
        // Arrange
        var password = new string('a', 1000);

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }

    [Test]
    public void Handle_WhenPasswordIsShort_ReturnsHashedPassword()
    {
        // Arrange
        var password = "a";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }

    [Test]
    public void Handle_WhenPasswordContainsUnicode_ReturnsHashedPassword()
    {
        // Arrange
        var password = "Password 日本語 🔐";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }

    [Test]
    public void Handle_VerifiedPasswordCanBeVerified()
    {
        // Arrange
        var password = "SecurePassword123!";

        // Act
        var hashedPassword = this.handler.Handle(password);

        // Assert
        var verifyHandler = new Fenicia.Auth.Domains.Security.VerifyPassword.VerifyPasswordHandler();
        var isValid = verifyHandler.Handle(password, hashedPassword);
        Assert.That(isValid, Is.True, "Hashed password should be verifiable");
    }

    [Test]
    public void Handle_WhenPasswordHasWhitespace_ReturnsHashedPassword()
    {
        // Arrange
        var password = "  password with spaces  ";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }
}

using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.Security.VerifyPassword;

namespace Fenicia.Auth.Tests.Domains.Security.VerifyPassword;

[TestFixture]
public class VerifyPasswordHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        this.faker = new Faker();
        this.handler = new VerifyPasswordHandler();
        this.hashPasswordHandler = new HashPasswordHandler();
    }

    private Faker faker = null!;
    private VerifyPasswordHandler handler = null!;
    private HashPasswordHandler hashPasswordHandler = null!;

    [Test]
    public void Handle_WhenPasswordMatchesHash_ReturnsTrue()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.That(result, Is.True, "Should return true when password matches hash");
    }

    [Test]
    public void Handle_WhenPasswordDoesNotMatchHash_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var wrongPassword = this.faker.Internet.Password();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(wrongPassword, hashedPassword);

        // Assert
        Assert.That(result, Is.False, "Should return false when password doesn't match");
    }

    [Test]
    public void Handle_WhenPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.hashPasswordHandler.Handle(this.faker.Internet.Password());

        // Act
        var result = this.handler.Handle(null!, hashedPassword);

        // Assert
        Assert.That(result, Is.False, "Should return false when password is null");
    }

    [Test]
    public void Handle_WhenHashedPasswordIsNull_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this.handler.Handle(password, null!);

        // Assert
        Assert.That(result, Is.False, "Should return false when hashed password is null");
    }

    [Test]
    public void Handle_WhenBothPasswordAndHashAreNull_ReturnsFalse()
    {
        // Act
        var result = this.handler.Handle(null!, null!);

        // Assert
        Assert.That(result, Is.False, "Should return false when both are null");
    }

    [Test]
    public void Handle_WhenPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var hashedPassword = this.hashPasswordHandler.Handle(this.faker.Internet.Password());

        // Act
        var result = this.handler.Handle(string.Empty, hashedPassword);

        // Assert
        Assert.That(result, Is.False, "Should return false when password is empty");
    }

    [Test]
    public void Handle_WhenHashedPasswordIsEmpty_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();

        // Act
        var result = this.handler.Handle(password, string.Empty);

        // Assert
        Assert.That(result, Is.False, "Should return false when hashed password is empty");
    }

    [Test]
    public void Handle_WhenPasswordHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var wrongCasePassword = password.ToLowerInvariant();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(wrongCasePassword, hashedPassword);

        // Assert
        Assert.That(result, Is.False, "Should return false when case doesn't match");
    }

    [Test]
    public void Handle_WhenHashIsInvalidFormat_ReturnsFalse()
    {
        // Arrange
        var password = this.faker.Internet.Password();
        var invalidHash = this.faker.Lorem.Word();

        // Act
        var result = this.handler.Handle(password, invalidHash);

        // Assert
        Assert.That(result, Is.False, "Should return false for invalid hash format");
    }

    [Test]
    public void Handle_WhenPasswordContainsSpecialCharacters_VerifiesCorrectly()
    {
        // Arrange
        var password = $"P@$$w0rd!{this.faker.Random.AlphaNumeric(10)}";
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.That(result, Is.True, "Should verify password with special characters");
    }

    [Test]
    public void Handle_WhenPasswordIsVeryLong_VerifiesCorrectly()
    {
        // Arrange
        var password = this.faker.Lorem.Paragraphs();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.That(result, Is.True, "Should verify very long password");
    }

    [Test]
    public void Handle_WhenPasswordIsShort_VerifiesCorrectly()
    {
        // Arrange
        var password = this.faker.Random.Char().ToString();
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.That(result, Is.True, "Should verify short password");
    }

    [Test]
    public void Handle_WhenPasswordContainsUnicode_VerifiesCorrectly()
    {
        // Arrange
        var password = $"{this.faker.Internet.Password()} 日本語 🔐";
        var hashedPassword = this.hashPasswordHandler.Handle(password);

        // Act
        var result = this.handler.Handle(password, hashedPassword);

        // Assert
        Assert.That(result, Is.True, "Should verify password with unicode characters");
    }

    [Test]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.True, "password1 should match hash1");
            Assert.That(result2, Is.False, "password1 should not match hash2");
            Assert.That(result3, Is.False, "password2 should not match hash1");
            Assert.That(result4, Is.True, "password2 should match hash2");
        }
    }
}
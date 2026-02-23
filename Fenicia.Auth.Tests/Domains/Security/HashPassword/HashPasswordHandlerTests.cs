using Bogus;

using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.Security.VerifyPassword;

namespace Fenicia.Auth.Tests.Domains.Security.HashPassword;

[TestFixture]
public class HashPasswordHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        this.faker = new Faker();
        this.handler = new HashPasswordHandler();
    }

    private Faker faker = null!;
    private HashPasswordHandler handler = null!;

    [Test]
    public void Handle_WhenValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = this.faker.Internet.Password();

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
        var password = this.faker.Internet.Password();

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
        var password = $"P@$$w0rd!{this.faker.Random.AlphaNumeric(10)}";

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
        var password = this.faker.Lorem.Paragraphs(5);

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
        var password = this.faker.Random.Char().ToString();

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
        var password = $"{this.faker.Internet.Password()} 日本語 🔐";

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
        var password = this.faker.Internet.Password();

        // Act
        var hashedPassword = this.handler.Handle(password);

        // Assert
        var verifyHandler = new VerifyPasswordHandler();
        var isValid = verifyHandler.Handle(password, hashedPassword);
        Assert.That(isValid, Is.True, "Hashed password should be verifiable");
    }

    [Test]
    public void Handle_WhenPasswordHasWhitespace_ReturnsHashedPassword()
    {
        // Arrange
        var password = $"  {this.faker.Internet.Password()} with spaces  ";

        // Act
        var result = this.handler.Handle(password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.EqualTo(password), "Hashed password should not equal plain text");
    }
}
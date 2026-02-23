using Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt.LoginAttempt;

[TestFixture]
public class LoginAttemptHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.handler = new LoginAttemptHandler(this.cache);
    }

    [TearDown]
    public void TearDown()
    {
        this.cache.Dispose();
    }

    private IMemoryCache cache = null!;
    private LoginAttemptHandler handler = null!;

    [Test]
    public void Handle_WhenNoAttemptsExist_ReturnsZero()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = this.handler.Handle(email);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 when no attempts exist");
    }

    [Test]
    public void Handle_WhenAttemptsExist_ReturnsAttemptCount()
    {
        // Arrange
        var email = "test@example.com";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 3);

        // Act
        var result = this.handler.Handle(email);

        // Assert
        Assert.That(result, Is.EqualTo(3), "Should return the correct attempt count");
    }

    [Test]
    public void Handle_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        // Arrange
        var email = "test@example.com";
        var upperCaseEmail = "TEST@EXAMPLE.COM";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 5);

        // Act
        var result = this.handler.Handle(upperCaseEmail);

        // Assert
        Assert.That(result, Is.EqualTo(5), "Should return correct count regardless of case");
    }

    [Test]
    public void Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this.handler.Handle(null!));
    }

    [Test]
    public void Handle_WhenEmailIsEmpty_ReturnsZero()
    {
        // Arrange
        var email = string.Empty;

        // Act
        var result = this.handler.Handle(email);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 for empty email");
    }

    [Test]
    public void Handle_WhenEmailContainsSpaces_ReturnsZero()
    {
        // Arrange
        var email = " test@example.com ";

        // Act
        var result = this.handler.Handle(email);

        // Assert
        Assert.That(result, Is.EqualTo(0), "Should return 0 for email with spaces (not normalized)");
    }

    [Test]
    public void Handle_WithMultipleDifferentEmails_ReturnsCorrectCounts()
    {
        // Arrange
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        this.cache.Set(key1, 2);
        this.cache.Set(key2, 4);

        // Act
        var result1 = this.handler.Handle(email1);
        var result2 = this.handler.Handle(email2);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.EqualTo(2), "Should return correct count for user1");
            Assert.That(result2, Is.EqualTo(4), "Should return correct count for user2");
        }
    }

    [Test]
    public void Handle_WhenAttemptCountIsHigh_ReturnsCorrectCount()
    {
        // Arrange
        var email = "test@example.com";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 100);

        // Act
        var result = this.handler.Handle(email);

        // Assert
        Assert.That(result, Is.EqualTo(100), "Should return high attempt count correctly");
    }
}
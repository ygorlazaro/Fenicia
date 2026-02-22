using Fenicia.Auth.Domains.LoginAttempt.ResetAttempts;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt.ResetAttempts;

[TestFixture]
public class ResetAttemptsHandlerTests
{
    private IMemoryCache cache = null!;
    private ResetAttemptsHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.handler = new ResetAttemptsHandler(this.cache);
    }

    [TearDown]
    public void TearDown()
    {
        this.cache.Dispose();
    }

    [Test]
    public async Task Handle_WhenAttemptsExist_RemovesAttempts()
    {
        // Arrange
        var email = "test@example.com";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 5);

        // Act
        await this.handler.Handle(email, CancellationToken.None);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.That(exists, Is.False, "Should remove the login attempts");
    }

    [Test]
    public async Task Handle_WhenNoAttemptsExist_CompletesSuccessfully()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        await this.handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.Pass("Should complete successfully even when no attempts exist");
    }

    [Test]
    public async Task Handle_WhenEmailHasDifferentCase_RemovesCorrectAttempts()
    {
        // Arrange
        var email = "test@example.com";
        var upperCaseEmail = "TEST@EXAMPLE.COM";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 3);

        // Act
        await this.handler.Handle(upperCaseEmail, CancellationToken.None);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.That(exists, Is.False, "Should remove attempts regardless of email case");
    }

    [Test]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.Handle(null!, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenEmailIsEmpty_RemovesEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 2);

        // Act
        await this.handler.Handle(email, CancellationToken.None);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.That(exists, Is.False, "Should remove attempts for empty email");
    }

    [Test]
    public async Task Handle_WhenMultipleEmailsExist_RemovesOnlySpecifiedEmail()
    {
        // Arrange
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        this.cache.Set(key1, 2);
        this.cache.Set(key2, 4);

        // Act
        await this.handler.Handle(email1, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.cache.TryGetValue(key1, out _), Is.False, "Should remove user1 attempts");
            Assert.That(this.cache.TryGetValue(key2, out int count), Is.True, "Should keep user2 attempts");
            Assert.That(count, Is.EqualTo(4), "User2 count should remain unchanged");
        }
    }

    [Test]
    public async Task Handle_WhenHighAttemptCountExists_RemovesSuccessfully()
    {
        // Arrange
        var email = "test@example.com";
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 100);

        // Act
        await this.handler.Handle(email, CancellationToken.None);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.That(exists, Is.False, "Should remove high attempt count");
    }

    [Test]
    public async Task Handle_MultipleResetsForSameEmail_CompletesSuccessfully()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        await this.handler.Handle(email, CancellationToken.None);
        await this.handler.Handle(email, CancellationToken.None);
        await this.handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.Pass("Should handle multiple resets without errors");
    }
}

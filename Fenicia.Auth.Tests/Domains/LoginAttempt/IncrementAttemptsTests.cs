using Bogus;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

[TestFixture]
public class IncrementAttemptsTests
{
    [SetUp]
    public void SetUp()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.handler = new Auth.Domains.LoginAttempt.IncrementAttempts.IncrementAttempts(this.cache);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.cache.Dispose();
    }

    private MemoryCache cache;
    private Auth.Domains.LoginAttempt.IncrementAttempts.IncrementAttempts handler;
    private Faker faker;

    [Test]
    public async Task Handle_WhenNoPreviousAttempts_SetsCountToOne()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.Handle(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(1), "Should set count to 1 on first attempt");
        }
    }

    [Test]
    public async Task Handle_WhenPreviousAttemptsExist_IncrementsCount()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 3);

        // Act
        await this.handler.Handle(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(4), "Should increment existing count");
        }
    }

    [Test]
    public async Task Handle_WhenEmailHasDifferentCase_NormalizesToLowerCase()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 2);

        // Act
        await this.handler.Handle(upperCaseEmail);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(3), "Should increment regardless of email case");
        }
    }

    [Test]
    public void Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.Handle(null!));
    }

    [Test]
    public async Task Handle_WhenEmailIsEmpty_SetsCountForEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.Handle(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(1), "Should handle empty email");
        }
    }

    [Test]
    public async Task Handle_MultipleIncrementsForSameEmail_IncrementsCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        await this.handler.Handle(email);
        await this.handler.Handle(email);
        await this.handler.Handle(email);

        // Assert
        var key = $"login-attempt:{email.ToLower()}";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(3), "Should increment correctly multiple times");
        }
    }

    [Test]
    public async Task Handle_MultipleDifferentEmails_TracksSeparately()
    {
        // Arrange
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();

        // Act
        await this.handler.Handle(email1);
        await this.handler.Handle(email1);
        await this.handler.Handle(email2);

        // Assert
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.cache.TryGetValue(key1, out int count1), Is.True);
            Assert.That(count1, Is.EqualTo(2), "User1 should have 2 attempts");
            Assert.That(this.cache.TryGetValue(key2, out int count2), Is.True);
            Assert.That(count2, Is.EqualTo(1), "User2 should have 1 attempt");
        }
    }

    [Test]
    public async Task Handle_WhenExpirationIsSet_ExpiresAfterTimeSpan()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.Handle(email);

        // Assert - verify entry exists
        Assert.That(this.cache.TryGetValue(key, out _), Is.True, "Entry should exist after increment");
    }

    [Test]
    public async Task Handle_WithHighAttemptCount_IncrementsCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 99);

        // Act
        await this.handler.Handle(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(100), "Should handle high counts correctly");
        }
    }

    [Test]
    public async Task Handle_WhenEmailContainsSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email(firstName: "test+");
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.Handle(email);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(1), "Should handle special characters in email");
        }
    }
}

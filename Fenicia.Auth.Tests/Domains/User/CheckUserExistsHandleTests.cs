using Bogus;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

[TestFixture]
public class CheckUserExistsHandleTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new CheckUserExistsHandle(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private CheckUserExistsHandle handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenEmailExists_ReturnsTrue()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true when email exists");
    }

    [Test]
    public async Task Handle_WhenEmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when email doesn't exist");
    }

    [Test]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var upperCaseEmail = "TEST@EXAMPLE.COM";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(upperCaseEmail, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Email comparison is case-sensitive");
    }

    [Test]
    public async Task Handle_WhenMultipleUsersExist_OnlyMatchesExactEmail()
    {
        // Arrange
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email1,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email2,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.AddRange(user1, user2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result1 = await this.handler.Handle(email1, CancellationToken.None);
        var result2 = await this.handler.Handle(email2, CancellationToken.None);
        var result3 = await this.handler.Handle("other@example.com", CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.True, "Should find user1");
            Assert.That(result2, Is.True, "Should find user2");
            Assert.That(result3, Is.False, "Should not find other user");
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false with empty database");
    }

    [Test]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var emailWithSpaces = " test@example.com ";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(emailWithSpaces, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should not match email with extra spaces");
    }

    [Test]
    public async Task Handle_WhenEmailHasExtraCharacters_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var emailWithExtra = "test@example.com.";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(emailWithExtra, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should not match email with extra characters");
    }
}
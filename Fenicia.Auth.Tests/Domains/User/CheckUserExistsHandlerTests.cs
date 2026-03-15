using Bogus;

using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CheckUserExistsHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CheckUserExistsHandler handler;

    public CheckUserExistsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new CheckUserExistsHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ReturnsTrue()
    {
        // Arrange
        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.True(result, "Should return true when email exists");
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = faker.Internet.Email();

        // Act
        var result = await handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.False(result, "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(upperCaseEmail, CancellationToken.None);

        // Assert
        Assert.False(result, "Email comparison is case-sensitive");
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_OnlyMatchesExactEmail()
    {
        // Arrange
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result1 = await handler.Handle(email1, CancellationToken.None);
        var result2 = await handler.Handle(email2, CancellationToken.None);
        var result3 = await handler.Handle("other@example.com", CancellationToken.None);

        // Assert
        Assert.True(result1, "Should find user1");
        Assert.True(result2, "Should find user2");
        Assert.False(result3, "Should not find other user");
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var email = faker.Internet.Email();

        // Act
        var result = await handler.Handle(email, CancellationToken.None);

        // Assert
        Assert.False(result, "Should return false with empty database");
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsFalse()
    {
        // Arrange
        const string email = "test@example.com";
        const string emailWithSpaces = " test@example.com ";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(emailWithSpaces, CancellationToken.None);

        // Assert
        Assert.False(result, "Should not match email with extra spaces");
    }

    [Fact]
    public async Task Handle_WhenEmailHasExtraCharacters_ReturnsFalse()
    {
        // Arrange
        const string email = "test@example.com";
        const string emailWithExtra = "test@example.com.";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(emailWithExtra, CancellationToken.None);

        // Assert
        Assert.False(result, "Should not match email with extra characters");
    }
}

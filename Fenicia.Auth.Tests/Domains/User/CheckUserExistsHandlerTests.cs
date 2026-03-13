using Bogus;

using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CheckUserExistsHandlerTests : IDisposable
{
    public CheckUserExistsHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.handler = new CheckUserExistsHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly CheckUserExistsHandler handler;
    private readonly Faker faker;

    [Fact]
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

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.True(result,
            "Should return true when email exists");
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.False(result,
            "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(upperCaseEmail,
            CancellationToken.None);

        // Assert
        Assert.False(result,
            "Email comparison is case-sensitive");
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

        this.db.AuthUsers.AddRange(user1,
            user2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result1 = await this.handler.Handle(email1,
            CancellationToken.None);
        var result2 = await this.handler.Handle(email2,
            CancellationToken.None);
        var result3 = await this.handler.Handle("other@example.com",
            CancellationToken.None);

        // Assert
        Assert.True(result1,
            "Should find user1");
        Assert.True(result2,
            "Should find user2");
        Assert.False(result3,
            "Should not find other user");
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.False(result,
            "Should return false with empty database");
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
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(emailWithSpaces,
            CancellationToken.None);

        // Assert
        Assert.False(result,
            "Should not match email with extra spaces");
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
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(emailWithExtra,
            CancellationToken.None);

        // Assert
        Assert.False(result,
            "Should not match email with extra characters");
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }
}

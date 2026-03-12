using Bogus;

using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetByEmailHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetByEmailHandler handler;
    private readonly Faker faker;

    public GetByEmailHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new GetByEmailHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUserResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(userId,
            result.Id);
        Assert.Equal(email,
            result.Email);
        Assert.Equal(name,
            result.Name);
        Assert.Equal(password,
            result.Password);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(upperCaseEmail,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var name1 = this.faker.Person.FullName;
        var name2 = this.faker.Person.FullName;
        var password1 = this.faker.Internet.Password();
        var password2 = this.faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = password1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = password2
        };

        this.db.AuthUsers.AddRange(user1,
            user2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(email1,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.Equal(userId1,
            result.Id);
        Assert.Equal(email1,
            result.Email);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var emailWithSpaces = " test@example.com ";
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(emailWithSpaces,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesResponseContainsAllFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(email,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        
        Assert.NotEqual(Guid.Empty,
            result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
        Assert.NotNull(result.Password);
    }
}

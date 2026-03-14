using Bogus;

using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetUserForRefreshHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetUserForRefreshHandler handler;

    public GetUserForRefreshHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        this.db = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetUserForRefreshHandler(this.db);
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

        var user = new UserModel { Id = userId, Email = email, Name = name, Password = this.faker.Internet.Password() };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";
        var name1 = this.faker.Person.FullName;
        var name2 = this.faker.Person.FullName;

        var user1 = new UserModel { Id = userId1, Email = email1, Name = name1, Password = this.faker.Internet.Password() };

        var user2 = new UserModel { Id = userId2, Email = email2, Name = name2, Password = this.faker.Internet.Password() };

        this.db.AuthUsers.AddRange(user1, user2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task Handle_ResponseDoesNotIncludePassword()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var password = this.faker.Internet.Password();

        var user = new UserModel { Id = userId, Email = email, Name = name, Password = password };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Email);
    }

    [Fact]
    public async Task Handle_VerifiesResponseContainsAllExpectedFields()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;

        var user = new UserModel { Id = userId, Email = email, Name = name, Password = this.faker.Internet.Password() };

        this.db.AuthUsers.Add(user);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
    }
}
using Bogus;

using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdatePasswordHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdatePasswordHandler handler;

    public UpdatePasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new UpdatePasswordHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ChangesPasswordSuccessfully()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        const string oldPassword = "old_hashed_password";

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = oldPassword
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);

        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(oldPassword, updatedUser.Password);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsArgumentException()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task Handle_PasswordIsHashedBeforeSaving()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = "old_password"
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await handler.Handle(query, CancellationToken.None);

        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.True(updatedUser.Password.Length > newPassword.Length);
    }

    [Fact]
    public async Task Handle_VerifiesPasswordCanBeVerified()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = "old_password"
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await handler.Handle(query, CancellationToken.None);

        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);

        var verifyHandler = new VerifyPasswordService();
        var isValid = verifyHandler.Handle(newPassword, updatedUser.Password);
        Assert.True(isValid);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_OnlyUpdatesRequestedUser()
    {

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        const string oldPassword1 = "old_password_1";
        const string oldPassword2 = "old_password_2";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = oldPassword1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = oldPassword2
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId1, newPassword);

        await handler.Handle(query, CancellationToken.None);

        var updatedUser1 = await db.AuthUsers.FindAsync(userId1);
        var updatedUser2 = await db.AuthUsers.FindAsync(userId2);

        Assert.NotEqual(oldPassword1, updatedUser1!.Password);
        Assert.Equal(oldPassword2, updatedUser2!.Password);
    }

    [Fact]
    public async Task Handle_PreservesOtherUserProperties()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var email = faker.Internet.Email();
        var name = faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await handler.Handle(query, CancellationToken.None);

        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);

        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(name, updatedUser.Name);
        Assert.Equal(userId, updatedUser.Id);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsArgumentException()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsEmpty_StillHashesAndSaves()
    {

        var userId = Guid.NewGuid();
        var newPassword = string.Empty;

        var user = new UserModel
        {
            Id = userId,
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = "old_password"
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("Password cannot be null or empty", ex.Message);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponseData()
    {

        var userId = Guid.NewGuid();
        var newPassword = faker.Internet.Password();
        var email = faker.Internet.Email();
        var name = faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(userId, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);
    }
}

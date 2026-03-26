using Bogus;

using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Auth.Domains.ForgotPassword.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

/// <summary>
///     Unit tests for the AddForgotPasswordHandler.
///     Tests the forgot password initiation logic including code generation and user validation.
/// </summary>
public class AddForgotPasswordHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddForgotPasswordHandler handler;

    public AddForgotPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new AddForgotPasswordHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that when an email exists, a forgot password code is created successfully.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailExists_CreatesForgotPasswordCodeSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(6, forgotPassword.Code.Length);
        Assert.True(forgotPassword.IsActive);
        Assert.Equal(userId, forgotPassword.UserId);
        Assert.True(forgotPassword.ExpirationDate > DateTime.UtcNow);
        Assert.Null(forgotPassword.IpAddress);
        Assert.Null(forgotPassword.UserAgent);
    }

    /// <summary>
    ///     Tests that when an email does not exist, ItemNotExistsException is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    /// <summary>
    ///     Tests that email matching is case-sensitive (different case throws exception).
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(upperCaseEmail);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    /// <summary>
    ///     Tests that when multiple users exist, the code is created for the correct user.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId1);
        Assert.NotNull(forgotPassword);
        Assert.Equal(userId1, forgotPassword.UserId);
        Assert.Equal(6, forgotPassword.Code.Length);

        var forgotPasswordForUser2 = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId2);
        Assert.Null(forgotPasswordForUser2);
    }

    /// <summary>
    ///     Tests that calling the handler multiple times creates multiple codes for the same user.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCalledMultipleTimesForSameUser_CreatesMultipleCodes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email);

        // Act
        await handler.Handle(command, CancellationToken.None);
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var codes = await db.AuthForgottenPasswords.Where(fp => fp.UserId == userId).ToListAsync();
        Assert.Equal(2, codes.Count);
        Assert.True(codes.All(c => c.IsActive));
        Assert.True(codes.All(c => c.Code.Length == 6));
    }

    /// <summary>
    ///     Tests that when the database is empty, ItemNotExistsException is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    /// <summary>
    ///     Tests that generated codes are unique across different users.
    /// </summary>
    [Fact]
    public async Task Handle_VerifiesCodeIsUnique()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.AddRange(user1, user2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command1 = new AddForgotPasswordCommand(email1);
        var command2 = new AddForgotPasswordCommand(email2);

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var codes = await db.AuthForgottenPasswords.ToListAsync();
        var distinctCodes = codes.Select(c => c.Code).Distinct().ToList();
        Assert.Equal(2, distinctCodes.Count);
    }

    [Fact]
    public async Task Handle_WhenIpAddressAndUserAgentProvided_StoresThemCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0 (Test Browser)";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddForgotPasswordCommand(email, ipAddress, userAgent);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var forgotPassword = await db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(ipAddress, forgotPassword.IpAddress);
        Assert.Equal(userAgent, forgotPassword.UserAgent);
    }
}

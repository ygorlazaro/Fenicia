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
///     Unit tests for the ResetPasswordHandler.
///     Tests the password reset logic including code validation, password update, and code invalidation.
/// </summary>
public class ResetPasswordHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ResetPasswordHandler handler;

    public ResetPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new ResetPasswordHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that a valid code successfully resets the user's password.
    /// </summary>
    [Fact]
    public async Task Handle_WhenValidCode_ResetsPasswordSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(faker.Internet.Password(), updatedUser.Password);

        var updatedCode = await db.AuthForgottenPasswords.FindAsync(forgotPassword.Id);
        Assert.NotNull(updatedCode);
        Assert.False(updatedCode.IsActive);
    }

    /// <summary>
    ///     Tests that when no user exists with the given email, ItemNotExistsException is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    /// <summary>
    ///     Tests that an invalid code throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var validCode = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        const string invalidCode = "INVALID";
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = validCode,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, invalidCode);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    /// <summary>
    ///     Tests that an inactive code throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCodeIsInactive_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = false,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    /// <summary>
    ///     Tests that an expired code throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCodeIsExpired_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(-10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    /// <summary>
    ///     Tests that a code belonging to a different user throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

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

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId1,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.AddRange(user1, user2);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email2, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    /// <summary>
    ///     Tests that using a code a second time throws InvalidDataException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCodeIsUsedSecondTime_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act - First use
        await handler.Handle(command, CancellationToken.None);

        // Act & Assert - Second use
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    /// <summary>
    ///     Tests that the password is actually changed after reset.
    /// </summary>
    [Fact]
    public async Task Handle_VerifiesPasswordWasActuallyChanged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        db.AuthUsers.Add(user);
        db.AuthForgottenPasswords.Add(forgotPassword);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(user.Password, updatedUser.Password);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(user.Name, updatedUser.Name);
    }

    /// <summary>
    ///     Tests that when the database is empty, ItemNotExistsException is thrown.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await handler.Handle(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }
}

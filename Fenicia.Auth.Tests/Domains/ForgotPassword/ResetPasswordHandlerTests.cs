using Bogus;

using Fenicia.Auth.Domains.ForgotPassword.ResetPassword;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ChangePassword;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ResetPasswordHandlerTests : IDisposable
{
    public ResetPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        var hashPasswordHandler = new HashPasswordHandler();
        var changePasswordHandler = new ChangePasswordHandler(this.context, hashPasswordHandler);
        this.handler = new ResetPasswordHandler(this.context, changePasswordHandler);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly ResetPasswordHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenValidCode_ResetsPasswordSuccessfully()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_hashed_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual("old_hashed_password", updatedUser.Password);

        var updatedCode = await this.context.AuthForgottenPasswords.FindAsync(forgotPassword.Id);
        Assert.NotNull(updatedCode);
        Assert.False(updatedCode.IsActive);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var validCode = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        const string invalidCode = "INVALID";
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = validCode,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, invalidCode);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsInactive_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = false,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsExpired_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(-10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = this.faker.Person.FullName,
            Password = "old_password1"
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = this.faker.Person.FullName,
            Password = "old_password2"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId1,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.AddRange(user1, user2);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email2, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsUsedSecondTime_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act - First use
        await this.handler.Handle(command, CancellationToken.None);

        // Act & Assert - Second use
        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesPasswordWasActuallyChanged()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = this.faker.Person.FullName,
            Password = "old_hashed_password"
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        this.context.AuthUsers.Add(user);
        this.context.AuthForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual("old_hashed_password", updatedUser.Password);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(user.Name, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal("User with given email does not exist.", ex.Message);
    }
}

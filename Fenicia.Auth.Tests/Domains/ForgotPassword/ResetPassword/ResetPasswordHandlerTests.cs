using Bogus;

using Fenicia.Auth.Domains.ForgotPassword.ResetPassword;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User.ChangePassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword.ResetPassword;

[TestFixture]
public class ResetPasswordHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        var hashPasswordHandler = new HashPasswordHandler();
        this.changePasswordHandler = new ChangePasswordHandler(this.context, hashPasswordHandler);
        this.handler = new ResetPasswordHandler(this.context, this.changePasswordHandler);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private ResetPasswordHandler handler = null!;
    private ChangePasswordHandler changePasswordHandler = null!;
    private Faker faker = null!;

    [Test]
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.Password, Is.Not.EqualTo("old_hashed_password"), "Password should be changed");

        var updatedCode = await this.context.ForgottenPasswords.FindAsync(forgotPassword.Id);
        Assert.That(updatedCode, Is.Not.Null);
        Assert.That(updatedCode!.IsActive, Is.False, "Code should be invalidated after use");
    }

    [Test]
    public void Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }

    [Test]
    public async Task Handle_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var validCode = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var invalidCode = "INVALID";
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, invalidCode);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid code"));
    }

    [Test]
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid code"));
    }

    [Test]
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid code"));
    }

    [Test]
    public async Task Handle_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
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

        this.context.Users.AddRange(user1, user2);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email2, newPassword, code);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid code"));
    }

    [Test]
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act - First use
        await this.handler.Handle(command, CancellationToken.None);

        // Act & Assert - Second use
        var ex = Assert.ThrowsAsync<InvalidDataException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Invalid code"));
    }

    [Test]
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

        this.context.Users.Add(user);
        this.context.ForgottenPasswords.Add(forgotPassword);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedUser = await this.context.Users.FindAsync(userId);
        Assert.That(updatedUser, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedUser!.Password, Is.Not.EqualTo("old_hashed_password"), "Password hash should change");
            Assert.That(updatedUser.Password, Is.Not.EqualTo(newPassword), "Password should be hashed, not plain text");
            Assert.That(updatedUser.Email, Is.EqualTo(email), "Email should not change");
            Assert.That(updatedUser.Name, Is.EqualTo(user.Name), "Name should not change");
        }
    }

    [Test]
    public void Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = this.faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ItemNotExistsException>(async () =>
            await this.handler.Handle(command, CancellationToken.None)
        );
        Assert.That(ex?.Message, Is.EqualTo("Item not found"));
    }
}
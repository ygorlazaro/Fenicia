using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ResetPasswordServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ForgotPasswordService service;

    public ResetPasswordServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        service = new ForgotPasswordService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidCode_ResetsPasswordSuccessfully()
    {

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

        await service.ResetAsync(command, CancellationToken.None);

        var updatedUser = await db.AuthUsers.FindAsync(userId);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(faker.Internet.Password(), updatedUser.Password);

        var updatedCode = await db.AuthForgottenPasswords.FindAsync(forgotPassword.Id);
        Assert.NotNull(updatedCode);
        Assert.False(updatedCode.IsActive);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {

        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {

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

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsInactive_ThrowsInvalidDataException()
    {

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

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsExpired_ThrowsInvalidDataException()
    {

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

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {

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

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsUsedSecondTime_ThrowsInvalidDataException()
    {

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

        await service.ResetAsync(command, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesPasswordWasActuallyChanged()
    {

        var userId = Guid.NewGuid();
        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var originalPassword = "OriginalPassword123!";
        var newPassword = "NewPassword456!";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = faker.Person.FullName,
            Password = originalPassword
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

        await service.ResetAsync(command, CancellationToken.None);

        db.ChangeTracker.Clear();
        var updatedUser = await db.AuthUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(user.Name, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {

        var email = faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }
}

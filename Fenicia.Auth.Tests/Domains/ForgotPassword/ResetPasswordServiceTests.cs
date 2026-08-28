using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ResetPasswordServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ForgotPasswordService _service;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public ResetPasswordServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository);
        _service = new ForgotPasswordService(_db, userService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidCode_ResetsPasswordSuccessfully()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _service.ResetAsync(command, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(_faker.Internet.Password(), updatedUser.Password);

        var updatedCode = await _db.AuthForgottenPasswords.FindAsync(forgotPassword.Id);
        Assert.NotNull(updatedCode);
        Assert.False(updatedCode.IsActive);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeDoesNotExist_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var validCode = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        const string invalidCode = "INVALID";
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = validCode,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, invalidCode);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsInactive_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = false,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsExpired_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(-10)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeBelongsToDifferentUser_ThrowsInvalidDataException()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId1,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        _db.AuthUsers.AddRange(user1, user2);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email2, newPassword, code);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCodeIsUsedSecondTime_ThrowsInvalidDataException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var forgotPassword = new ForgotPasswordModel
        {
            Id = Guid.NewGuid(),
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(10)
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _service.ResetAsync(command, CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidDataException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("Invalid forgot password code.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesPasswordWasActuallyChanged()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var originalPassword = "OriginalPassword123!";
        var newPassword = "NewPassword456!";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
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

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.AuthForgottenPasswords.Add(forgotPassword);
        _db.SaveChanges();

        var command = new ResetPasswordCommand(email, newPassword, code);

        await _service.ResetAsync(command, CancellationToken.None);

        _db.ChangeTracker.Clear();
        var updatedUser = await _db.AuthUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(user.Name, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var code = Guid.NewGuid().ToString().Replace("-", string.Empty)[..6];
        var newPassword = _faker.Internet.Password();

        var command = new ResetPasswordCommand(email, newPassword, code);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.ResetAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }
}

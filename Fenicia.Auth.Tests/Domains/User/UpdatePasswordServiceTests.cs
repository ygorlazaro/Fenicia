using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UpdatePasswordServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public UpdatePasswordServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository);
        _userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserExists_ChangesPasswordSuccessfully()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string oldPassword = "old_hashed_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(oldPassword, updatedUser.Password);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_PasswordIsHashedBeforeSaving()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.True(updatedUser.Password.Length > newPassword.Length);
    }

    [Fact]
    public async Task UpdatePasswordAsync_VerifiesPasswordCanBeVerified()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        var isValid = new SecurityService().Verify(newPassword, updatedUser.Password);
        Assert.True(isValid);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenMultipleUsersExist_OnlyUpdatesRequestedUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string oldPassword1 = "old_password_1";
        const string oldPassword2 = "old_password_2";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword2
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId1, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser1 = await _userRepository.GetByIdAsync(userId1, CancellationToken.None).ContinueWith(t => t.Result);
        var updatedUser2 = await _userRepository.GetByIdAsync(userId2, CancellationToken.None).ContinueWith(t => t.Result);

        Assert.NotEqual(oldPassword1, updatedUser1!.Password);
        Assert.Equal(oldPassword2, updatedUser2!.Password);
    }

    [Fact]
    public async Task UpdatePasswordAsync_PreservesOtherUserProperties()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(name, updatedUser.Name);
        Assert.Equal(userId, updatedUser.Id);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WithEmptyDatabase_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenPasswordIsEmpty_StillHashesAndSaves()
    {
        var userId = Guid.NewGuid();
        var newPassword = string.Empty;

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("Password cannot be null or empty", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ReturnsCorrectResponseData()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        Assert.Equal(userId, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);
    }
}

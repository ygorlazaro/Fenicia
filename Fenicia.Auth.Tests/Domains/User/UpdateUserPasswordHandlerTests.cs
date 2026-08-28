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

public class UpdateUserPasswordHandlerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;
    private readonly UserModel _testUser;

    public UpdateUserPasswordHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        _userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository);
        _faker = new Faker();

        _testUser = new UserModel
        {
            Email = _faker.Internet.Email(),
            Password = SecurityService.Hash(_faker.Internet.Password()),
            Name = _faker.Person.FullName
        };

        _userRepository.InsertAsync(_testUser, CancellationToken.None).GetAwaiter().GetResult();
        _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ChangesPasswordSuccessfully()
    {
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(_testUser.Id, newPassword);
        var originalPasswordHash = _testUser.Password;

        var result = await _userService.UpdatePasswordAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.True(result.Success);
        Assert.Equal("Password changed successfully", result.Message);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(originalPasswordHash, updatedUser.Password);
    }

    [Fact]
    public async Task Handle_NewPasswordIsHashed()
    {
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(_testUser.Id, newPassword);

        await _userService.UpdatePasswordAsync(request, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.StartsWith("$2", updatedUser.Password);

        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(nonExistentUserId, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdatePasswordAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }
}

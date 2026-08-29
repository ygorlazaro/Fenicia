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

public class UpdateUserHandlerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;
    private readonly UserModel _testUser;

    public UpdateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        _userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository, new SecurityService());
        _faker = new Faker();

        _testUser = new UserModel
        {
            Email = _faker.Internet.Email(),
            Password = new SecurityService().Hash(_faker.Internet.Password()),
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
    public async Task Handle_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        var newName = _faker.Person.FullName;
        var request = new UpdateUserCommand(_testUser.Id, newName);

        var result = await _userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        var newEmail = _faker.Internet.Email();
        var request = new UpdateUserCommand(_testUser.Id, Email: newEmail);

        var result = await _userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newEmail, result.Email);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserCommand(nonExistentUserId, "Test");

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var existingEmail = _faker.Internet.Email();

        var anotherUser = new UserModel
        {
            Email = existingEmail,
            Password = new SecurityService().Hash(_faker.Internet.Password()),
            Name = _faker.Person.FullName
        };

        await _userRepository.InsertAsync(anotherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserCommand(_testUser.Id, Email: existingEmail);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        var role = new RoleModel { Name = "Admin" };
        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
        new(Guid.NewGuid(), role.Id)
        };

        var request = new UpdateUserCommand(_testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        var company = new CompanyModel
        {
            Name = _faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
        new(company.Id, Guid.NewGuid())
        };

        var request = new UpdateUserCommand(_testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }
}

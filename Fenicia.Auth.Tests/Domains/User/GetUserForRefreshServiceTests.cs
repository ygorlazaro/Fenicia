using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetUserForRefreshServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public GetUserForRefreshServiceTests()
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
    public async Task GetForRefreshAsync_WhenUserExists_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
    }

    [Fact]
    public async Task GetForRefreshAsync_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.GetForRefreshAsync(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task GetForRefreshAsync_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";
        var name1 = _faker.Person.FullName;
        var name2 = _faker.Person.FullName;

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task GetForRefreshAsync_WithEmptyDatabase_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.GetForRefreshAsync(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task GetForRefreshAsync_ResponseDoesNotIncludePassword()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Email);
    }

    [Fact]
    public async Task GetForRefreshAsync_VerifiesResponseContainsAllExpectedFields()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
    }
}

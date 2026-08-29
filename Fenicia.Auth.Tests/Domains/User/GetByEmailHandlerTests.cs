using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class GetByEmailHandlerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public GetByEmailHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        _userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository, new SecurityService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUserResponse()
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

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(password, result.Password);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsNull()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
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

        var result = await _userService.GetByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var name1 = _faker.Person.FullName;
        var name2 = _faker.Person.FullName;
        var password1 = _faker.Internet.Password();
        var password2 = _faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = password1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = password2
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(email1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var emailWithSpaces = " test@example.com ";
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

        var result = await _userService.GetByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesResponseContainsAllFields()
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

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
        Assert.NotNull(result.Password);
    }
}

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
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class CheckUserExistsHandlerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public CheckUserExistsHandlerTests()
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
    public async Task Handle_WhenEmailExists_ReturnsTrue()
    {
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.True(result, "Should return true when email exists");
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ReturnsFalse()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ReturnsFalse()
    {
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.False(result, "Email comparison is case-sensitive");
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_OnlyMatchesExactEmail()
    {
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result1 = await _userService.ExistsByEmailAsync(email1, CancellationToken.None);
        var result2 = await _userService.ExistsByEmailAsync(email2, CancellationToken.None);
        var result3 = await _userService.ExistsByEmailAsync("other@example.com", CancellationToken.None);

        Assert.True(result1, "Should find user1");
        Assert.True(result2, "Should find user2");
        Assert.False(result3, "Should not find other user");
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false with empty database");
    }

    [Fact]
    public async Task Handle_WhenEmailContainsExtraSpaces_ReturnsFalse()
    {
        const string email = "test@example.com";
        const string emailWithSpaces = " test@example.com ";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.False(result, "Should not match email with extra spaces");
    }

    [Fact]
    public async Task Handle_WhenEmailHasExtraCharacters_ReturnsFalse()
    {
        const string email = "test@example.com";
        const string emailWithExtra = "test@example.com.";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(emailWithExtra, CancellationToken.None);

        Assert.False(result, "Should not match email with extra characters");
    }
}

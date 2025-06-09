using Bogus;
using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class UserRepositoryTests
{
    private AuthContext _context;
    private UserRepository _sut;
    private DbContextOptions<AuthContext> _options;
    private Faker<UserModel> _userGenerator;
    private Faker<CompanyModel> _companyGenerator;
    private Faker<UserRoleModel> _userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new UserRepository(_context);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _userGenerator = new Faker<UserModel>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Password, f => f.Internet.Password());

        _companyGenerator = new Faker<CompanyModel>()
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Company.CompanyName())
            .RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##.###.###/####-##"));

        _userRoleGenerator = new Faker<UserRoleModel>()
            .RuleFor(ur => ur.Id, f => Guid.NewGuid())
            .RuleFor(ur => ur.UserId, f => Guid.NewGuid())
            .RuleFor(ur => ur.CompanyId, f => Guid.NewGuid());
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetByEmailAndCnpjAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = _userGenerator.Generate();
        var company = _companyGenerator.Generate();
        var userRole = _userRoleGenerator
            .Clone()
            .RuleFor(ur => ur.UserId, user.Id)
            .RuleFor(ur => ur.CompanyId, company.Id)
            .Generate();

        await _context.Users.AddAsync(user);
        await _context.Companies.AddAsync(company);
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByEmailAndCnpjAsync(user.Email, company.Cnpj);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(user.Id));
        Assert.That(result.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task GetByEmailAndCnpjAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentEmail = "nonexistent@example.com";
        var nonExistentCnpj = "00.000.000/0000-00";

        // Act
        var result = await _sut.GetByEmailAndCnpjAsync(nonExistentEmail, nonExistentCnpj);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void Add_ShouldAddUserToContext()
    {
        // Arrange
        var user = _userGenerator.Generate();

        // Act
        var result = _sut.Add(user);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(_context.Users.Local, Does.Contain(user));
    }

    [Test]
    public async Task SaveAsync_ShouldPersistChangesToDatabase()
    {
        // Arrange
        var user = _userGenerator.Generate();
        _sut.Add(user);

        // Act
        var saveResult = await _sut.SaveAsync();

        // Assert
        Assert.That(saveResult, Is.GreaterThan(0));
        var savedUser = await _context.Users.FindAsync(user.Id);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task CheckUserExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var user = _userGenerator.Generate();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.CheckUserExistsAsync(user.Email);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckUserExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await _sut.CheckUserExistsAsync(nonExistentEmail);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUserForRefreshTokenAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = _userGenerator.Generate();
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetUserForRefreshTokenAsync(user.Id);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(user.Id));
        Assert.That(result.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task GetUserForRefreshTokenAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await _sut.GetUserForRefreshTokenAsync(nonExistentUserId);

        // Assert
        Assert.That(result, Is.Null);
    }
}

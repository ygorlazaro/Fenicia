namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.User;

public class UserRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private Faker<CompanyModel> _companyGenerator;
    private AuthContext _context;
    private DbContextOptions<AuthContext> _options;
    private UserRepository _sut;
    private Faker<UserModel> _userGenerator;
    private Faker<UserRoleModel> _userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<UserRepository>>().Object;
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        _context = new AuthContext(_options);
        _sut = new UserRepository(_context, mockLogger);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _userGenerator = new Faker<UserModel>().RuleFor(u => u.Id, _ => Guid.NewGuid()).RuleFor(u => u.Email, f => f.Internet.Email()).RuleFor(u => u.Name, f => f.Name.FullName()).RuleFor(u => u.Password, f => f.Internet.Password());

        _companyGenerator = new Faker<CompanyModel>().RuleFor(c => c.Id, _ => Guid.NewGuid()).RuleFor(c => c.Name, f => f.Company.CompanyName()).RuleFor(c => c.Cnpj, f => f.Random.ReplaceNumbers("##.###.###/####-##"));

        _userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
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
        var userRole = _userRoleGenerator.Clone().RuleFor(ur => ur.UserId, user.Id).RuleFor(ur => ur.CompanyId, company.Id).Generate();

        await _context.Users.AddAsync(user, _cancellationToken);
        await _context.Companies.AddAsync(company, _cancellationToken);
        await _context.UserRoles.AddAsync(userRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetByEmailAndCnpjAsync(user.Email, company.Cnpj, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        });
    }

    [Test]
    public async Task GetByEmailAndCnpjAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        const string nonExistentEmail = "nonexistent@example.com";
        const string nonExistentCnpj = "00.000.000/0000-00";

        // Act
        var result = await _sut.GetByEmailAndCnpjAsync(nonExistentEmail, nonExistentCnpj, _cancellationToken);

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

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(_context.Users.Local, Does.Contain(user));
        });
    }

    [Test]
    public async Task SaveAsync_ShouldPersistChangesToDatabase()
    {
        // Arrange
        var user = _userGenerator.Generate();
        _sut.Add(user);

        // Act
        var saveResult = await _sut.SaveAsync(_cancellationToken);

        // Assert
        Assert.That(saveResult, Is.GreaterThan(expected: 0));
        var savedUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, _cancellationToken);
        Assert.That(savedUser, Is.Not.Null);
        Assert.That(savedUser!.Email, Is.EqualTo(user.Email));
    }

    [Test]
    public async Task CheckUserExistsAsync_WhenUserExists_ReturnsTrue()
    {
        // Arrange
        var user = _userGenerator.Generate();
        await _context.Users.AddAsync(user, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.CheckUserExistsAsync(user.Email, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task CheckUserExistsAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var nonExistentEmail = "nonexistent@example.com";

        // Act
        var result = await _sut.CheckUserExistsAsync(nonExistentEmail, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task GetUserForRefreshTokenAsync_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var user = _userGenerator.Generate();
        await _context.Users.AddAsync(user, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetUserForRefreshTokenAsync(user.Id, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result!.Id, Is.EqualTo(user.Id));
            Assert.That(result.Email, Is.EqualTo(user.Email));
        });
    }

    [Test]
    public async Task GetUserForRefreshTokenAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var result = await _sut.GetUserForRefreshTokenAsync(nonExistentUserId, _cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }
}

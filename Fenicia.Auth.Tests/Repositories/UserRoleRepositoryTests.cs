namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Domains.UserRole.Logic;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

public class UserRoleRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private AuthContext _context;
    private DbContextOptions<AuthContext> _options;
    private Faker<RoleModel> _roleGenerator;
    private UserRoleRepository _sut;
    private Faker<UserRoleModel> _userRoleGenerator;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<UserRoleRepository>>().Object;
        _context = new AuthContext(_options);
        _sut = new UserRoleRepository(_context, mockLogger);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _roleGenerator = new Faker<RoleModel>().RuleFor(r => r.Id, _ => Guid.NewGuid()).RuleFor(r => r.Name, f => f.Name.JobTitle());

        _userRoleGenerator = new Faker<UserRoleModel>().RuleFor(ur => ur.Id, _ => Guid.NewGuid()).RuleFor(ur => ur.UserId, _ => Guid.NewGuid()).RuleFor(ur => ur.Role, _ => _roleGenerator.Generate()).RuleFor(ur => ur.CompanyId, _ => Guid.NewGuid());
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasRoles_ReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roles = new[] { "Admin", "User", "Manager" };
        var userRoles = roles.Select(role => _userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.Role, _ => _roleGenerator.Clone().RuleFor(r => r.Name, role).Generate()).Generate()).ToList();

        await _context.UserRoles.AddRangeAsync(userRoles, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetRolesByUserAsync(userId, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Has.Length.EqualTo(roles.Length));
        Assert.That(result, Is.EquivalentTo(roles));
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.GetRolesByUserAsync(userId, _cancellationToken);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExistsInCompanyAsync_WhenUserExistsInCompany_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRole = _userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).Generate();

        await _context.UserRoles.AddAsync(userRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.ExistsInCompanyAsync(userId, companyId, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ExistsInCompanyAsync_WhenUserDoesNotExistInCompany_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        // Act
        var result = await _sut.ExistsInCompanyAsync(userId, companyId, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserHasRoleInCompany_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = _userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => _roleGenerator.Clone().RuleFor(r => r.Name, roleName).Generate()).Generate();

        await _context.UserRoles.AddAsync(userRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, roleName, _cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserDoesNotHaveRoleInCompany_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";
        var userRole = _userRoleGenerator.Clone().RuleFor(ur => ur.UserId, userId).RuleFor(ur => ur.CompanyId, companyId).RuleFor(ur => ur.Role, _ => _roleGenerator.Clone().RuleFor(r => r.Name, "DifferentRole").Generate()).Generate();

        await _context.UserRoles.AddAsync(userRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, roleName, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserDoesNotExistInCompany_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleName = "Admin";

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, roleName, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }
}

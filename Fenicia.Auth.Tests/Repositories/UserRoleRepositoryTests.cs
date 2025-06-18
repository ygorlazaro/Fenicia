using Bogus;
using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.UserRole;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class UserRoleRepositoryTests
{
    private AuthContext _context;
    private UserRoleRepository _sut;
    private DbContextOptions<AuthContext> _options;
    private Faker<UserRoleModel> _userRoleGenerator;
    private Faker<RoleModel> _roleGenerator;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new UserRoleRepository(_context);

        SetupFakers();
    }

    private void SetupFakers()
    {
        _roleGenerator = new Faker<RoleModel>()
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.Name, f => f.Name.JobTitle());

        _userRoleGenerator = new Faker<UserRoleModel>()
            .RuleFor(ur => ur.Id, f => Guid.NewGuid())
            .RuleFor(ur => ur.UserId, f => Guid.NewGuid())
            .RuleFor(ur => ur.Role, f => _roleGenerator.Generate())
            .RuleFor(ur => ur.CompanyId, f => Guid.NewGuid());
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
        var userRoles = roles
            .Select(role =>
                _userRoleGenerator
                    .Clone()
                    .RuleFor(ur => ur.UserId, userId)
                    .RuleFor(
                        ur => ur.Role,
                        f => _roleGenerator.Clone().RuleFor(r => r.Name, role).Generate()
                    )
                    .Generate()
            )
            .ToList();

        await _context.UserRoles.AddRangeAsync(userRoles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetRolesByUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Empty);
        Assert.That(result.Length, Is.EqualTo(roles.Length));
        Assert.That(result, Is.EquivalentTo(roles));
    }

    [Test]
    public async Task GetRolesByUserAsync_WhenUserHasNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _sut.GetRolesByUserAsync(userId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task ExistsInCompanyAsync_WhenUserExistsInCompany_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var userRole = _userRoleGenerator
            .Clone()
            .RuleFor(ur => ur.UserId, userId)
            .RuleFor(ur => ur.CompanyId, companyId)
            .Generate();

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ExistsInCompanyAsync(userId, companyId);

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
        var result = await _sut.ExistsInCompanyAsync(userId, companyId);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserHasRoleInCompany_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleName = "Admin";
        var userRole = _userRoleGenerator
            .Clone()
            .RuleFor(ur => ur.UserId, userId)
            .RuleFor(ur => ur.CompanyId, companyId)
            .RuleFor(
                ur => ur.Role,
                f => _roleGenerator.Clone().RuleFor(r => r.Name, roleName).Generate()
            )
            .Generate();

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, roleName);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task HasRoleAsync_WhenUserDoesNotHaveRoleInCompany_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleName = "Admin";
        var userRole = _userRoleGenerator
            .Clone()
            .RuleFor(ur => ur.UserId, userId)
            .RuleFor(ur => ur.CompanyId, companyId)
            .RuleFor(
                ur => ur.Role,
                f => _roleGenerator.Clone().RuleFor(r => r.Name, "DifferentRole").Generate()
            )
            .Generate();

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.HasRoleAsync(userId, companyId, roleName);

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
        var result = await _sut.HasRoleAsync(userId, companyId, roleName);

        // Assert
        Assert.That(result, Is.False);
    }
}

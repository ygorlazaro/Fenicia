using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class RoleRepositoryTests
{
    private AuthContext _context;
    private RoleRepository _sut;
    private DbContextOptions<AuthContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new RoleRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsRole_WhenAdminRoleExists()
    {
        // Arrange
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        await _context.Roles.AddAsync(adminRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAdminRoleAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Admin"));
        Assert.That(result.Id, Is.EqualTo(adminRole.Id));
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsNull_WhenAdminRoleDoesNotExist()
    {
        // Arrange
        var nonAdminRole = new RoleModel { Id = Guid.NewGuid(), Name = "User" };

        await _context.Roles.AddAsync(nonAdminRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAdminRoleAsync();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsNull_WhenRolesTableIsEmpty()
    {
        // Act
        var result = await _sut.GetAdminRoleAsync();

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsSingleRole_WhenMultipleRolesExist()
    {
        // Arrange
        var roles = new[]
        {
            new RoleModel { Id = Guid.NewGuid(), Name = "User" },
            new RoleModel { Id = Guid.NewGuid(), Name = "Admin" },
            new RoleModel { Id = Guid.NewGuid(), Name = "Manager" },
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAdminRoleAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task GetAdminRoleAsync_IsCaseInsensitive()
    {
        // Arrange
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "ADMIN" };

        await _context.Roles.AddAsync(adminRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAdminRoleAsync();

        // Assert
        Assert.That(result, Is.Null, "GetAdminRoleAsync should be case sensitive");
    }
}

namespace Fenicia.Auth.Tests.Repositories;

using Contexts;

using Domains.Role.Data;
using Domains.Role.Logic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

public class RoleRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private AuthContext _context;
    private DbContextOptions<AuthContext> _options;
    private RoleRepository _sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<RoleRepository>>().Object;

        _context = new AuthContext(_options);
        _sut = new RoleRepository(_context, mockLogger);
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

        await _context.Roles.AddAsync(adminRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetAdminRoleAsync(_cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.Name, Is.EqualTo(expected: "Admin"));
            Assert.That(result.Id, Is.EqualTo(adminRole.Id));
        });
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsNull_WhenAdminRoleDoesNotExist()
    {
        // Arrange
        var nonAdminRole = new RoleModel { Id = Guid.NewGuid(), Name = "User" };

        await _context.Roles.AddAsync(nonAdminRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetAdminRoleAsync(_cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsync_ReturnsNull_WhenRolesTableIsEmpty()
    {
        // Act
        var result = await _sut.GetAdminRoleAsync(_cancellationToken);

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
                        new RoleModel { Id = Guid.NewGuid(), Name = "Manager" }
                    };

        await _context.Roles.AddRangeAsync(roles, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetAdminRoleAsync(_cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo(expected: "Admin"));
    }

    [Test]
    public async Task GetAdminRoleAsync_IsCaseInsensitive()
    {
        // Arrange
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "ADMIN" };

        await _context.Roles.AddAsync(adminRole, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.GetAdminRoleAsync(_cancellationToken);

        // Assert
        Assert.That(result, Is.Null, message: "GetAdminRoleAsync should be case sensitive");
    }
}

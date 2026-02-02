using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace Fenicia.Auth.Tests.Repositories;

public class RoleRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private DbContextOptions<AuthContext> options;
    private RoleRepository sut;

    [SetUp]
    public void Setup()
    {
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        var mockLogger = new Mock<ILogger<RoleRepository>>().Object;

        this.context = new AuthContext(this.options);
        this.sut = new RoleRepository(this.context, mockLogger);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsRoleWhenAdminRoleExists()
    {
        // Arrange
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        await this.context.Roles.AddAsync(adminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo("Admin"));
            Assert.That(result.Id, Is.EqualTo(adminRole.Id));
        }
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsNullWhenAdminRoleDoesNotExist()
    {
        // Arrange
        var nonAdminRole = new RoleModel { Id = Guid.NewGuid(), Name = "User" };

        await this.context.Roles.AddAsync(nonAdminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsNullWhenRolesTableIsEmpty()
    {
        // Act
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsSingleRoleWhenMultipleRolesExist()
    {
        // Arrange
        var roles = new[]
                    {
                        new RoleModel { Id = Guid.NewGuid(), Name = "User" },
                        new RoleModel { Id = Guid.NewGuid(), Name = "Admin" },
                        new RoleModel { Id = Guid.NewGuid(), Name = "Manager" }
                    };

        await this.context.Roles.AddRangeAsync(roles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task GetAdminRoleAsyncIsCaseInsensitive()
    {
        // Arrange
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "ADMIN" };

        await this.context.Roles.AddAsync(adminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        // Assert
        Assert.That(result, Is.Null, "GetAdminRoleAsync should be case sensitive");
    }
}

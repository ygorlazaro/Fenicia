using Fenicia.Auth.Domains.UserRole.GetRolesByUser;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole.GetRolesByUser;

[TestFixture]
public class GetRolesByUserHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new GetRolesByUserHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private GetRolesByUserHandler handler = null!;

    [Test]
    public async Task Handler_WhenUserHasRoles_ReturnsRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var role = new RoleModel
        {
            Id = roleId,
            Name = "Admin"
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = Guid.NewGuid(),
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.EqualTo(1), "Should return 1 role");
        Assert.That(result[0], Is.EqualTo("Admin"), "Role name should be Admin");
    }

    [Test]
    public async Task Handler_WhenUserHasNoRoles_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty array");
    }

    [Test]
    public async Task Handler_WhenUserHasMultipleRoles_ReturnsAllRoles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        };

        var userRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var managerRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "Manager"
        };

        this.context.Roles.AddRange(adminRole, userRole, managerRole);

        var userRoles = new List<UserRoleModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyId = companyId,
                RoleId = adminRole.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyId = companyId,
                RoleId = userRole.Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CompanyId = companyId,
                RoleId = managerRole.Id
            }
        };

        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.EqualTo(3), "Should return 3 roles");
            Assert.That(result, Does.Contain("Admin"));
            Assert.That(result, Does.Contain("User"));
            Assert.That(result, Does.Contain("Manager"));
        }
    }

    [Test]
    public async Task Handler_WhenMultipleUsersExist_ReturnsOnlyRequestedUserRoles()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "Admin"
        };

        var userRole = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        this.context.Roles.AddRange(adminRole, userRole);

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            CompanyId = companyId,
            RoleId = adminRole.Id
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId2,
            CompanyId = companyId,
            RoleId = userRole.Id
        };

        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetRolesByUserQuery(userId1);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Length, Is.EqualTo(1), "Should return only user1's roles");
            Assert.That(result[0], Is.EqualTo("Admin"), "Should return Admin role for user1");
        }
    }

    [Test]
    public async Task Handler_WithEmptyDatabase_ReturnsEmptyArray()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty, "Should return empty array for empty database");
    }

    [Test]
    public async Task Handler_WhenUserHasSameRoleInDifferentCompanies_ReturnsRoleOncePerCompany()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        this.context.Roles.Add(adminRole);

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = Guid.NewGuid(),
            RoleId = adminRoleId
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = Guid.NewGuid(),
            RoleId = adminRoleId
        };

        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Length, Is.EqualTo(2), "Should return role for each company");
        Assert.That(result.Count(r => r == "Admin"), Is.EqualTo(2), "Should have Admin twice");
    }

    [Test]
    public async Task Handler_VerifiesResultContainsOnlyRoleNames()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        var roles = new List<RoleModel>
        {
            new() { Id = Guid.NewGuid(), Name = "Role1" },
            new() { Id = Guid.NewGuid(), Name = "Role2" },
            new() { Id = Guid.NewGuid(), Name = "Role3" }
        };

        this.context.Roles.AddRange(roles);

        var userRoles = roles.Select(r => new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = r.Id
        }).ToList();

        this.context.UserRoles.AddRange(userRoles);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetRolesByUserQuery(userId);

        // Act
        var result = await this.handler.Handler(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.All(r => !string.IsNullOrEmpty(r)), Is.True, "All role names should be non-empty");
            Assert.That(result.Length, Is.EqualTo(3), "Should have 3 roles");
        }
    }
}
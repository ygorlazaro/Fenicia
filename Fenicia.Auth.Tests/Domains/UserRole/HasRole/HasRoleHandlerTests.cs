using Fenicia.Auth.Domains.UserRole.HasRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.UserRole.HasRole;

[TestFixture]
public class HasRoleHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.handler = new HasRoleHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private AuthContext context = null!;
    private HasRoleHandler handler = null!;

    [Test]
    public async Task Handle_WhenUserHasRole_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new HasRoleQuery(userId, companyId, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true when user has the role");
    }

    [Test]
    public async Task Handle_WhenUserDoesNotHaveRole_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";
        const string otherRoleName = "User";

        var role = new RoleModel
        {
            Id = roleId,
            Name = otherRoleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new HasRoleQuery(userId, companyId, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when user doesn't have the role");
    }

    [Test]
    public async Task Handle_WhenUserHasRoleInDifferentCompany_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId1 = Guid.NewGuid();
        var companyId2 = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId1,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new HasRoleQuery(userId, companyId2, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when role is in different company");
    }

    [Test]
    public async Task Handle_WhenUserHasNoRoles_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";

        var query = new HasRoleQuery(userId, companyId, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when user has no roles");
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "Admin";

        var query = new HasRoleQuery(userId, companyId, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false for empty database");
    }

    [Test]
    public async Task Handle_WhenRoleNameHasDifferentCase_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new HasRoleQuery(userId, companyId, "admin");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when case doesn't match");
    }

    [Test]
    public async Task Handle_WhenUserHasMultipleRoles_ReturnsTrueForCorrectRole()
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

        var query = new HasRoleQuery(userId, companyId, "Manager");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.True, "Should return true for Manager role");
    }

    [Test]
    public async Task Handle_WhenRoleNameHasExtraSpaces_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.Add(userRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new HasRoleQuery(userId, companyId, " Admin ");

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false when role name has extra spaces");
    }

    [Test]
    public async Task Handle_WhenMultipleUsersHaveSameRole_ReturnsTrueForCorrectUser()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        const string roleName = "Admin";

        var role = new RoleModel
        {
            Id = roleId,
            Name = roleName
        };

        var userRole1 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId1,
            CompanyId = companyId,
            RoleId = roleId
        };

        var userRole2 = new UserRoleModel
        {
            Id = Guid.NewGuid(),
            UserId = userId2,
            CompanyId = companyId,
            RoleId = roleId
        };

        this.context.Roles.Add(role);
        this.context.UserRoles.AddRange(userRole1, userRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query1 = new HasRoleQuery(userId1, companyId, roleName);
        var query2 = new HasRoleQuery(userId2, companyId, roleName);

        // Act
        var result1 = await this.handler.Handle(query1, CancellationToken.None);
        var result2 = await this.handler.Handle(query2, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result1, Is.True, "Should return true for user1");
            Assert.That(result2, Is.True, "Should return true for user2");
        }
    }

    [Test]
    public async Task Handle_WhenRoleDoesNotExistInDatabase_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        const string roleName = "NonExistentRole";

        var query = new HasRoleQuery(userId, companyId, roleName);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.False, "Should return false for non-existent role");
    }
}
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Role.GetAdminRole;

[TestFixture]
public class GetAdminRoleHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetAdminRoleHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
    private GetAdminRoleHandler handler = null!;

    [Test]
    public async Task Handle_WhenAdminRoleExists_ReturnsAdminRole()
    {
        // Arrange
        var adminRoleId = Guid.NewGuid();

        var adminRole = new AuthRole
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        this.context.Roles.Add(adminRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Id, Is.EqualTo(adminRoleId), "RoleId should match");
            Assert.That(result.Name, Is.EqualTo("Admin"), "RoleName should be Admin");
        }
    }

    [Test]
    public async Task Handle_WhenAdminRoleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var role = new AuthRole
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        this.context.Roles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WhenMultipleRolesExist_ReturnsOnlyAdminRole()
    {
        // Arrange
        var adminRoleId = Guid.NewGuid();

        var adminRole = new AuthRole
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        var userRole = new AuthRole
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        var managerRole = new AuthRole
        {
            Id = Guid.NewGuid(),
            Name = "Manager"
        };

        this.context.Roles.AddRange(adminRole, userRole, managerRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result!.Id, Is.EqualTo(adminRoleId), "Should return admin role");
            Assert.That(result.Name, Is.EqualTo("Admin"), "RoleName should be Admin");
        }
    }

    [Test]
    public async Task Handle_WhenAdminRoleNameHasDifferentCase_ReturnsNull()
    {
        // Arrange
        var role = new AuthRole
        {
            Id = Guid.NewGuid(),
            Name = "admin"
        };

        this.context.Roles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WhenAdminRoleNameHasExtraSpaces_ReturnsNull()
    {
        // Arrange
        var role = new AuthRole
        {
            Id = Guid.NewGuid(),
            Name = " Admin "
        };

        this.context.Roles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WhenMultipleAdminRolesExist_ReturnsFirst()
    {
        // Arrange
        var adminRoleId1 = Guid.NewGuid();
        var adminRoleId2 = Guid.NewGuid();

        var adminRole1 = new AuthRole
        {
            Id = adminRoleId1,
            Name = "Admin"
        };

        var adminRole2 = new AuthRole
        {
            Id = adminRoleId2,
            Name = "Admin"
        };

        this.context.Roles.AddRange(adminRole1, adminRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("Admin"), "Should return an Admin role");
    }
}

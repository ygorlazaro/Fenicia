using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Role;

public class GetAdminRoleHandlerTests : IDisposable
{
    public GetAdminRoleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.handler = new GetAdminRoleHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetAdminRoleHandler handler;

    [Fact]
    public async Task Handle_WhenAdminRoleExists_ReturnsAdminRole()
    {
        // Arrange
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        this.context.AuthRoles.Add(adminRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adminRoleId, result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleDoesNotExist_ReturnsNull()
    {
        // Arrange
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleRolesExist_ReturnsOnlyAdminRole()
    {
        // Arrange
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
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

        this.context.AuthRoles.AddRange(adminRole, userRole, managerRole);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(adminRoleId, result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNameHasDifferentCase_ReturnsNull()
    {
        // Arrange
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "admin"
        };

        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNameHasExtraSpaces_ReturnsNull()
    {
        // Arrange
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = " Admin "
        };

        this.context.AuthRoles.Add(role);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleAdminRolesExist_ReturnsFirst()
    {
        // Arrange
        var adminRoleId1 = Guid.NewGuid();
        var adminRoleId2 = Guid.NewGuid();

        var adminRole1 = new RoleModel
        {
            Id = adminRoleId1,
            Name = "Admin"
        };

        var adminRole2 = new RoleModel
        {
            Id = adminRoleId2,
            Name = "Admin"
        };

        this.context.AuthRoles.AddRange(adminRole1, adminRole2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }
}

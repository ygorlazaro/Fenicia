using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Role;

public class GetAdminRoleHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly RoleService service;

    public GetAdminRoleHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        service = new RoleService(db);
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleExists_ReturnsAdminRole()
    {

        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        db.AuthRoles.Add(adminRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(adminRoleId, result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleDoesNotExist_ReturnsNull()
    {

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        db.AuthRoles.Add(role);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleRolesExist_ReturnsOnlyAdminRole()
    {

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

        db.AuthRoles.AddRange(adminRole, userRole, managerRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNameHasDifferentCase_ReturnsNull()
    {

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "admin"
        };

        db.AuthRoles.Add(role);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNameHasExtraSpaces_ReturnsNull()
    {

        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = " Admin "
        };

        db.AuthRoles.Add(role);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WhenMultipleAdminRolesExist_ReturnsFirst()
    {

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

        db.AuthRoles.AddRange(adminRole1, adminRole2);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }
}

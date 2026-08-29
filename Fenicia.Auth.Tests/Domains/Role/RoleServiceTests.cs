using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Role;

public class RoleServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly RoleService _service;

    public RoleServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new RoleService(new RoleRepository(_db));
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleExists_ReturnsAdminRole()
    {
        var adminRoleId = Guid.NewGuid();

        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };

        _db.AuthRoles.Add(adminRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(adminRoleId, result.Id);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleDoesNotExist_ReturnsNull()
    {
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "User"
        };

        _db.AuthRoles.Add(role);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenMultipleRolesExist_ReturnsOnlyAdminRole()
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

        _db.AuthRoles.AddRange(adminRole, userRole, managerRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleNameHasDifferentCase_ReturnsNull()
    {
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = "admin"
        };

        _db.AuthRoles.Add(role);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WithEmptyDatabase_ReturnsNull()
    {
        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenAdminRoleNameHasExtraSpaces_ReturnsNull()
    {
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = " Admin "
        };

        _db.AuthRoles.Add(role);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAdminAsync_WhenMultipleAdminRolesExist_ReturnsFirst()
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

        _db.AuthRoles.AddRange(adminRole1, adminRole2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAdminAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Admin", result.Name);
    }
}

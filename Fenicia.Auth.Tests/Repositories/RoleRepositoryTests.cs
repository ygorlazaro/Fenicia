using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

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
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        context = new AuthContext(options);
        sut = new RoleRepository(context);
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsRoleWhenAdminRoleExists()
    {
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        await context.Roles.AddAsync(adminRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetAdminRoleAsync(cancellationToken);

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
        var nonAdminRole = new RoleModel { Id = Guid.NewGuid(), Name = "User" };

        await context.Roles.AddAsync(nonAdminRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetAdminRoleAsync(cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsNullWhenRolesTableIsEmpty()
    {
        var result = await sut.GetAdminRoleAsync(cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsSingleRoleWhenMultipleRolesExist()
    {
        var roles = new[]
                    {
                        new RoleModel { Id = Guid.NewGuid(), Name = "User" },
                        new RoleModel { Id = Guid.NewGuid(), Name = "Admin" },
                        new RoleModel { Id = Guid.NewGuid(), Name = "Manager" }
                    };

        await context.Roles.AddRangeAsync(roles, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetAdminRoleAsync(cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task GetAdminRoleAsyncIsCaseInsensitive()
    {
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "ADMIN" };

        await context.Roles.AddAsync(adminRole, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = await sut.GetAdminRoleAsync(cancellationToken);

        Assert.That(result, Is.Null, "GetAdminRoleAsync should be case sensitive");
    }
}

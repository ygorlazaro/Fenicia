using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;
        this.context = new AuthContext(this.options);
        this.sut = new RoleRepository(this.context);
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
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "Admin" };

        await this.context.Roles.AddAsync(adminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

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

        await this.context.Roles.AddAsync(nonAdminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAdminRoleAsyncReturnsNullWhenRolesTableIsEmpty()
    {
        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

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

        await this.context.Roles.AddRangeAsync(roles, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Admin"));
    }

    [Test]
    public async Task GetAdminRoleAsyncIsCaseInsensitive()
    {
        var adminRole = new RoleModel { Id = Guid.NewGuid(), Name = "ADMIN" };

        await this.context.Roles.AddAsync(adminRole, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        var result = await this.sut.GetAdminRoleAsync(this.cancellationToken);

        Assert.That(result, Is.Null, "GetAdminRoleAsync should be case sensitive");
    }
}
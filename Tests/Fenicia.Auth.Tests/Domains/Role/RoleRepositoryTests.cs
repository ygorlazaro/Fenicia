using Bogus;
using Fenicia.Auth.Domains.Role;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Role;

public class RoleRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly RoleRepository _repository;

    public RoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new RoleRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleExists_ReturnsRole()
    {
        var roleName = _faker.Random.Word();
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = roleName
        };

        _db.AuthRoles.Add(role);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByNameAsync(roleName, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(roleName, result.Name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByNameAsync("NonExistentRole", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_WhenRoleIsDeleted_ReturnsNull()
    {
        var roleName = _faker.Random.Word();
        var role = new RoleModel
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            Deleted = DateTime.UtcNow
        };

        _db.AuthRoles.Add(role);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByNameAsync(roleName, CancellationToken.None);

        Assert.Null(result);
    }
}
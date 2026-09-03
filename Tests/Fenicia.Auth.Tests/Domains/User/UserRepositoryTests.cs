using Bogus;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Tests.Domains.Security;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class UserRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new UserRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUser()
    {
        var email = _faker.Internet.Email();
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password())
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByEmailAsync("nonexistent@example.com", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserIsDeleted_ReturnsNull()
    {
        var email = _faker.Internet.Email();
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password()),
            Deleted = DateTime.UtcNow
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailExists_ReturnsTrue()
    {
        var email = _faker.Internet.Email();
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password())
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ReturnsFalse()
    {
        var result = await _repository.ExistsByEmailAsync("nonexistent@example.com", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenUserIsDeleted_ReturnsFalse()
    {
        var email = _faker.Internet.Email();
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = new TestSecurityService().Hash(_faker.Internet.Password()),
            Deleted = DateTime.UtcNow
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result);
    }
}
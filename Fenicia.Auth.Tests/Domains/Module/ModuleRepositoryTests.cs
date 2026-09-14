using Bogus;
using Fenicia.Auth.Domains.Module;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Module;

public class ModuleRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ModuleRepository _repository;

    public ModuleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new ModuleRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllActiveAsync_WhenModulesExist_ReturnsPaginatedActiveModules()
    {
        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m,
            IsActive = true,
            SortOrder = 1
        };

        var activeModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        var inactiveModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = false,
            SortOrder = 3
        };

        _db.AuthModules.AddRange(authModule, activeModule, inactiveModule);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(ModuleType.Basic, result[0].Type);
    }

    [Fact]
    public async Task GetAllActiveAsync_ResultsAreOrderedBySortOrder()
    {
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 1
        };

        _db.AuthModules.AddRange(module1, module2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(ModuleType.SocialNetwork, result[0].Type);
        Assert.Equal(ModuleType.Basic, result[1].Type);
    }

    [Fact]
    public async Task GetAllActiveAsync_WhenPageExceedsTotal_ReturnsEmptyList()
    {
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        _db.AuthModules.Add(module);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllActiveAsync(2, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CountAllActiveAsync_WhenModulesExist_ReturnsCorrectCount()
    {
        var authModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Auth,
            Price = 50.0m,
            IsActive = true,
            SortOrder = 1
        };

        var activeModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        var inactiveModule = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = false,
            SortOrder = 3
        };

        _db.AuthModules.AddRange(authModule, activeModule, inactiveModule);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAllActiveAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task CountAllActiveAsync_WhenNoModulesExist_ReturnsZero()
    {
        var result = await _repository.CountAllActiveAsync(CancellationToken.None);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task GetByIdsAsync_WhenIdsExist_ReturnsMatchingModulesOrderedByType()
    {
        var module1 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.SocialNetwork,
            Price = 20.0m,
            IsActive = true,
            SortOrder = 1
        };

        var module2 = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 2
        };

        _db.AuthModules.AddRange(module1, module2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdsAsync([module1.Id, module2.Id], CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(ModuleType.Basic, result[0].Type);
        Assert.Equal(ModuleType.SocialNetwork, result[1].Type);
    }

    [Fact]
    public async Task GetByIdsAsync_WhenSomeIdsDoNotExist_ReturnsOnlyExistingModules()
    {
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        _db.AuthModules.Add(module);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdsAsync([module.Id, Guid.NewGuid()], CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(module.Id, result[0].Id);
    }

    [Fact]
    public async Task GetByIdsAsync_WhenNoIdsExist_ReturnsEmptyList()
    {
        var result = await _repository.GetByIdsAsync([Guid.NewGuid()], CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTypeAsync_WhenModuleExists_ReturnsModule()
    {
        var module = new ModuleModel
        {
            Id = Guid.NewGuid(),
            Name = _faker.Commerce.ProductName(),
            Type = ModuleType.Basic,
            Price = 10.0m,
            IsActive = true,
            SortOrder = 1
        };

        _db.AuthModules.Add(module);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByTypeAsync(ModuleType.Basic, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(module.Id, result.Id);
    }

    [Fact]
    public async Task GetByTypeAsync_WhenModuleDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByTypeAsync(ModuleType.Basic, CancellationToken.None);

        Assert.Null(result);
    }
}
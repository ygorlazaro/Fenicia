using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Data.Tests.Contexts;
using Fenicia.Common.Data.Tests.Models;
using Fenicia.Common.Tests;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Tests.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly TestDataContext _db;
    private readonly Faker _faker;
    private readonly Repository<TestEntity> _repository;

    public RepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new TestDataContext(options, companyContext);
        _repository = new Repository<TestEntity>(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task InsertAsync_ShouldSetCreatedAndReturnEntity()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };

        var result = await _repository.InsertAsync(entity, CancellationToken.None);

        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(entity.Name);
        result.Created.Should().BeOnOrAfter(DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(entity.Id, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be(entity.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResults()
    {
        _db.TestEntities.Add(new TestEntity { Name = "A" });
        _db.TestEntities.Add(new TestEntity { Name = "B" });
        _db.TestEntities.Add(new TestEntity { Name = "C" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(1, 2, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity_WhenExists()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        var updated = new TestEntity { Id = entity.Id, Name = "Updated" };
        var result = await _repository.UpdateAsync(entity.Id, updated, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated");
        result.Updated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        var updated = new TestEntity { Name = "Updated" };
        var result = await _repository.UpdateAsync(Guid.NewGuid(), updated, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteEntity()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(entity.Id, CancellationToken.None);

        result.Should().Be(1);
        var deleted = await _db.TestEntities.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == entity.Id);
        deleted.Should().NotBeNull();
        deleted!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnZero_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_WithMultipleIds_ShouldSoftDeleteAll()
    {
        var entity1 = new TestEntity { Name = _faker.Commerce.ProductName() };
        var entity2 = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity1);
        _db.TestEntities.Add(entity2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(new[] { entity1.Id, entity2.Id }, CancellationToken.None);

        result.Should().Be(2);
        (await _db.TestEntities.IgnoreQueryFilters().CountAsync(e => e.Deleted != null)).Should().Be(2);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        _db.TestEntities.Add(new TestEntity { Name = "Alpha" });
        _db.TestEntities.Add(new TestEntity { Name = "Beta" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(e => e.Name == "Beta", CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenMatchExists()
    {
        _db.TestEntities.Add(new TestEntity { Name = "Alpha" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(e => e.Name == "Alpha", CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalse_WhenNoMatch()
    {
        var result = await _repository.AnyAsync(e => e.Name == "Alpha", CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnTotalCount()
    {
        _db.TestEntities.Add(new TestEntity { Name = "A" });
        _db.TestEntities.Add(new TestEntity { Name = "B" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        result.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        _db.TestEntities.Add(new TestEntity { Name = "Alpha" });
        _db.TestEntities.Add(new TestEntity { Name = "Beta" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(e => e.Name == "Beta", CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity);
        await _repository.SaveChangesAsync(CancellationToken.None);

        var count = await _db.TestEntities.CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task Query_ShouldReturnQueryable()
    {
        _db.TestEntities.Add(new TestEntity { Name = "Alpha" });
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = _repository.Query();

        var result = await query.ToListAsync();

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task InsertRangeAsync_ShouldInsertMultipleEntities()
    {
        var entities = new[]
        {
            new TestEntity { Name = "A" },
            new TestEntity { Name = "B" },
            new TestEntity { Name = "C" }
        };

        await _repository.InsertRangeAsync(entities, CancellationToken.None);

        var count = await _db.TestEntities.CountAsync();
        count.Should().Be(3);
    }

    [Fact]
    public async Task GetAllAsync_ShouldExcludeSoftDeletedEntities()
    {
        var entity = new TestEntity { Name = _faker.Commerce.ProductName() };
        _db.TestEntities.Add(entity);
        await _db.SaveChangesAsync(CancellationToken.None);
        await _repository.DeleteAsync(entity.Id, CancellationToken.None);

        var result = await _repository.GetAllAsync(page: 1, perPage: 10, CancellationToken.None);

        result.Should().BeEmpty();
    }
}

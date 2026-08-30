using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PositionRepository _repository;

    public PositionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new PositionRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllPositions()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(position.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenPositionIsValid_InsertsPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };

        var result = await _repository.InsertAsync(position, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_UpdatesPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(position.Id, position, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };

        var result = await _repository.UpdateAsync(position.Id, position, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_SoftDeletesPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(position.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedPosition = await _db.BasicPositions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == position.Id);
        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingPositions()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(p => p.Id == position.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenPositionExists_ReturnsTrue()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(p => p.Id == position.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Name.JobTitle() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(p => p.Id == position.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}

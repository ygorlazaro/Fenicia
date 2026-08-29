using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class PositionServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly PositionService _service;

    public PositionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new PositionRepository(_db);
        _service = new PositionService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenPositionsExist_ReturnsPaginationWithPositions()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllPositionQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetPositionByIdQuery(position.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetPositionByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesPosition()
    {
        var command = new AddPositionCommand(Guid.NewGuid(), _faker.Commerce.Department());

        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionExists_UpdatesPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdatePositionCommand(position.Id, "Updated Name");

        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var command = new UpdatePositionCommand(Guid.NewGuid(), "Updated Name");

        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenPositionExists_SoftDeletesPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeletePositionCommand(position.Id), _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var deletedPosition = await _db.BasicPositions.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == position.Id);
        Assert.NotNull(deletedPosition);
        Assert.NotNull(deletedPosition.Deleted);
    }
}

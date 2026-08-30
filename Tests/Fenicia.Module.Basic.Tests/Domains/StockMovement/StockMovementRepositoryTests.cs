using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.StockMovement;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly StockMovementRepository _repository;

    public StockMovementRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new StockMovementRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllStockMovements()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStockMovementExists_ReturnsStockMovement()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(stockMovement.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(stockMovement.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStockMovementDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenStockMovementIsValid_InsertsStockMovement()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };

        var result = await _repository.InsertAsync(stockMovement, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenStockMovementExists_UpdatesStockMovement()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(stockMovement.Id, stockMovement, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(stockMovement.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenStockMovementDoesNotExist_ReturnsNull()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };

        var result = await _repository.UpdateAsync(stockMovement.Id, stockMovement, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenStockMovementExists_SoftDeletesStockMovement()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(stockMovement.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedStockMovement = await _db.BasicStockMovements.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == stockMovement.Id);
        Assert.NotNull(deletedStockMovement);
        Assert.NotNull(deletedStockMovement.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingStockMovements()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(s => s.Id == stockMovement.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenStockMovementExists_ReturnsTrue()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(s => s.Id == stockMovement.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(s => s.Id == stockMovement.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}

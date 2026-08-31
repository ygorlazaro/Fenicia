using AwesomeAssertions;
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
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStockMovementExists_ReturnsStockMovement()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(stockMovement.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(stockMovement.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStockMovementDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenStockMovementIsValid_InsertsStockMovement()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };

        // Act
        var result = await _repository.InsertAsync(stockMovement, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenStockMovementExists_UpdatesStockMovement()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(stockMovement.Id, stockMovement, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(stockMovement.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenStockMovementDoesNotExist_ReturnsNull()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };

        // Act
        var result = await _repository.UpdateAsync(stockMovement.Id, stockMovement, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenStockMovementExists_SoftDeletesStockMovement()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(stockMovement.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedStockMovement = await _db.BasicStockMovements.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == stockMovement.Id);
        deletedStockMovement.Should().NotBeNull();
        deletedStockMovement!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingStockMovements()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(s => s.Id == stockMovement.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenStockMovementExists_ReturnsTrue()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(s => s.Id == stockMovement.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var stockMovement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = _faker.Random.Double(), Price = _faker.Random.Decimal(), Type = StockMovementType.In };
        _db.BasicStockMovements.Add(stockMovement);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(s => s.Id == stockMovement.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}

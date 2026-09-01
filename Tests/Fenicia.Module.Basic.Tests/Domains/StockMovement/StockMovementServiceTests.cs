using AwesomeAssertions;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementServiceTests : IDisposable
{
    private readonly Mock<IStockMovementRepository> _mockRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly StockMovementService _service;

    public StockMovementServiceTests()
    {
        _mockRepository = new Mock<IStockMovementRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _service = new StockMovementService(_mockRepository.Object, _mockProductRepository.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesMovement()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = "Test", SalesPrice = 10m, Quantity = 5 };
        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.UtcNow, 5m, StockMovementType.In, product.Id, null, null, null, null, "Test");
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<StockMovementModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovementModel m, CancellationToken _) => m);
        _mockProductRepository.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockProductRepository.Setup(r => r.UpdateAsync(product.Id, It.IsAny<ProductModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, ProductModel p, CancellationToken _) => p);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenMovementExists_UpdatesMovement()
    {
        // Arrange
        var movement = new StockMovementModel { Id = Guid.NewGuid(), Quantity = 5, Type = StockMovementType.In };
        _mockRepository.Setup(r => r.GetByIdAsync(movement.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(movement);
        _mockRepository.Setup(r => r.UpdateAsync(movement.Id, It.IsAny<StockMovementModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, StockMovementModel m, CancellationToken _) => m);

        var command = new UpdateStockMovementCommand(movement.Id, 10, DateTime.UtcNow, 6m, StockMovementType.Out, Guid.NewGuid(), null, null, null, null, "Updated");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(movement.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenMovementDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovementModel?)null);

        var command = new UpdateStockMovementCommand(Guid.NewGuid(), 10, DateTime.UtcNow, 6m, StockMovementType.Out, Guid.NewGuid(), null, null, null, null, "Updated");

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsDashboard()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetWithDetailsForDashboardAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetDashboardAsync(new GetStockMovementDashboardQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByDateRangeAsync_ReturnsList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}

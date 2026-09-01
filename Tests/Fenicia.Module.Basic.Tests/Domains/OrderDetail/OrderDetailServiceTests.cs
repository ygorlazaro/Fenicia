using AwesomeAssertions;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class OrderDetailServiceTests : IDisposable
{
    private readonly Mock<IOrderDetailRepository> _mockRepository;
    private readonly OrderDetailService _service;

    public OrderDetailServiceTests()
    {
        _mockRepository = new Mock<IOrderDetailRepository>();
        _service = new OrderDetailService(_mockRepository.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByOrderIdAsync_WhenDetailsExist_ReturnsResponses()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var product = new ProductModel { Id = Guid.NewGuid(), Name = "Test", SalesPrice = 10m };
        var details = new List<OrderDetailModel>
        {
            new() { Id = Guid.NewGuid(), OrderId = orderId, ProductId = product.Id, Product = product, Quantity = 2, Price = 10m }
        };

        _mockRepository.Setup(r => r.GetByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        // Act
        var result = await _service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(orderId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetDetailCountsByOrderIdsAsync_ReturnsDictionary()
    {
        // Arrange
        var orderIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var counts = orderIds.ToDictionary(id => id, _ => 3);

        _mockRepository.Setup(r => r.GetDetailCountsByOrderIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(counts);

        // Act
        var result = await _service.GetDetailCountsByOrderIdsAsync(orderIds, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetQuantitySumsByOrderIdsAsync_ReturnsDictionary()
    {
        // Arrange
        var orderIds = new List<Guid> { Guid.NewGuid() };
        var sums = orderIds.ToDictionary(id => id, _ => 5.0);

        _mockRepository.Setup(r => r.GetQuantitySumsByOrderIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(sums);

        // Act
        var result = await _service.GetQuantitySumsByOrderIdsAsync(orderIds, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByOrderDateRangeAsync_ReturnsList()
    {
        // Arrange
        var details = new List<OrderDetailModel>
        {
            new() { Id = Guid.NewGuid(), Quantity = 1, Price = 5m }
        };

        _mockRepository.Setup(r => r.GetByOrderDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        // Act
        var result = await _service.GetByOrderDateRangeAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByDateRangeAsync_ReturnsList()
    {
        // Arrange
        var details = new List<OrderDetailModel>
        {
            new() { Id = Guid.NewGuid(), Quantity = 4, Price = 7m }
        };

        _mockRepository.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(details);

        // Act
        var result = await _service.GetByDateRangeAsync(DateTime.UtcNow.AddDays(-30), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }
}

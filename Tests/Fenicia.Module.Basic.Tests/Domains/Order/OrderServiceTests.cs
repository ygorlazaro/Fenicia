using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderServiceTests : IDisposable
{
    private readonly Faker _faker;
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IStockMovementService> _mockStockMovementService;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        var mockOrderDetailService = new Mock<IOrderDetailService>();
        _mockStockMovementService = new Mock<IStockMovementService>();
        _service = new OrderService(
            _mockRepository.Object,
            mockOrderDetailService.Object,
            _mockStockMovementService.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel { Id = Guid.NewGuid(), Name = "Cust" }
        };
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = _faker.Random.Replace("ORD-########"),
            UserId = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            TotalAmount = _faker.Random.Decimal(),
            DiscountAmount = _faker.Random.Decimal(),
            TotalQuantity = _faker.Random.Int(),
            SaleDate = _faker.Date.Recent(),
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.Cash
        };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(order.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        // Act
        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesOrder()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            OrderStatus.Pending,
            [],
            PaymentMethod.Cash);
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<OrderModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderModel o, CancellationToken _) => o);
        _mockStockMovementService.Setup(s => s.AddAsync(
                It.IsAny<AddStockMovementCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddStockMovementCommand cmd, Guid _, CancellationToken _) =>
                new AddStockMovementResponse(
                    cmd.Id,
                    cmd.ProductId,
                    cmd.Quantity,
                    cmd.Date,
                    cmd.Price,
                    cmd.Type,
                    cmd.CustomerId,
                    cmd.SupplierId,
                    cmd.EmployeeId,
                    cmd.OrderId,
                    cmd.Reason));

        // Act
        var result = await _service.CreateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_DeletesOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeleteOrderCommand(orderId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsAnalytics()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAnalyticsOrdersAsync(
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetAnalyticsAsync(new GetOrderAnalyticsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTotalRevenueAsync_ReturnsTotal()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetTotalRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1234.56m);

        // Act
        var result = await _service.GetTotalRevenueAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1234.56m);
    }
}
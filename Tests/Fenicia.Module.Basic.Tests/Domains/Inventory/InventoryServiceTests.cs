using AwesomeAssertions;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail.Interfaces;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.StockMovement.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryServiceTests : IDisposable
{
    private readonly Mock<ICustomerService> _mockCustomerService;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ISupplierService> _mockSupplierService;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _mockProductService = new Mock<IProductService>();
        var mockStockMovementService = new Mock<IStockMovementService>();
        var mockOrderDetailService = new Mock<IOrderDetailService>();
        _mockCustomerService = new Mock<ICustomerService>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        _mockSupplierService = new Mock<ISupplierService>();
        _service = new InventoryService(
            _mockProductService.Object,
            mockStockMovementService.Object,
            mockOrderDetailService.Object,
            _mockCustomerService.Object,
            _mockEmployeeService.Object,
            _mockSupplierService.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_ReturnsInventory()
    {
        // Arrange
        _mockProductService.Setup(p => p.GetAllWithCategoryAsync(
                It.IsAny<GetAllProductQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockProductService.Setup(p => p.GetTotalCostPriceAsync(It.IsAny<CancellationToken>())).ReturnsAsync(100m);
        _mockProductService.Setup(p => p.GetTotalSalesPriceAsync(It.IsAny<CancellationToken>())).ReturnsAsync(200m);
        _mockProductService.Setup(p => p.GetTotalQuantityAsync(It.IsAny<CancellationToken>())).ReturnsAsync(50);

        // Act
        var result = await _service.GetAsync(new GetInventoryQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCostPrice.Should().Be(100m);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsDashboard()
    {
        // Arrange
        _mockProductService.Setup(p => p.GetLowStockAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockCustomerService.Setup(c => c.GetCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _mockEmployeeService.Setup(e => e.GetTotalEmployeesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _mockProductService.Setup(p => p.GetTotalCostValueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1000m);
        _mockProductService.Setup(p => p.GetTotalSalesValueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2000m);
        _mockProductService.Setup(p => p.GetTotalQuantityAsync(It.IsAny<CancellationToken>())).ReturnsAsync(100);
        _mockProductService.Setup(p => p.GetCategoryBreakdownAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockSupplierService.Setup(s => s.GetSupplierBreakdownAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act
        var result = await _service.GetDashboardAsync(new GetInventoryDashboardQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCustomers.Should().Be(10);
    }
}
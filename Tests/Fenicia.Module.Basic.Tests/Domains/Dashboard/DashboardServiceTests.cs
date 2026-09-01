using AwesomeAssertions;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class DashboardServiceTests : IDisposable
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockProductService = new Mock<IProductService>();
        var mockEmployeeService = new Mock<IEmployeeService>();
        _service = new DashboardService(_mockOrderService.Object, _mockProductService.Object, mockEmployeeService.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_ReturnsDashboard()
    {
        // Arrange
        _mockOrderService.Setup(o => o.GetTotalRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1000m);
        _mockOrderService.Setup(o => o.GetTotalCostAsync(It.IsAny<CancellationToken>())).ReturnsAsync(600m);
        _mockOrderService.Setup(o => o.GetTotalOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _mockOrderService.Setup(o => o.GetOrderWeeksAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockOrderService.Setup(o => o.GetTopCustomerOrdersAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockOrderService.Setup(o => o.GetOrderDatesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockOrderService.Setup(o => o.GetTodayRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(100m);
        _mockOrderService.Setup(o => o.GetTodayOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockOrderService.Setup(o => o.GetWeekRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(500m);
        _mockOrderService.Setup(o => o.GetWeekOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _mockOrderService.Setup(o => o.GetMonthRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(800m);
        _mockOrderService.Setup(o => o.GetMonthOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(8);
        _mockOrderService.Setup(o => o.GetLastMonthRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(700m);
        _mockOrderService.Setup(o => o.GetPendingAmountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(200m);
        _mockOrderService.Setup(o => o.GetPendingOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);
        _mockOrderService.Setup(o => o.GetApprovedAmountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(300m);
        _mockOrderService.Setup(o => o.GetApprovedOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(3);
        _mockProductService.Setup(p => p.GetTotalProductsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(50);

        // Act
        var result = await _service.GetFinancialDashboardAsync(new GetFinancialDashboardQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTotalRevenueAsync_ReturnsTotal()
    {
        // Arrange
        _mockOrderService.Setup(o => o.GetTotalRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(123m);

        // Act
        var result = await _service.GetTotalRevenueAsync(CancellationToken.None);

        // Assert
        result.Should().Be(123m);
    }

    [Fact]
    public async Task GetTotalOrdersAsync_ReturnsTotal()
    {
        // Arrange
        _mockOrderService.Setup(o => o.GetTotalOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(7);

        // Act
        var result = await _service.GetTotalOrdersAsync(CancellationToken.None);

        // Assert
        result.Should().Be(7);
    }
}

using AwesomeAssertions;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class DataSourceServiceTests : IDisposable
{
    private readonly Mock<ICustomerService> _mockCustomerService;
    private readonly Mock<IEmployeeService> _mockEmployeeService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ISupplierService> _mockSupplierService;
    private readonly DataSourceService _service;

    public DataSourceServiceTests()
    {
        _mockCustomerService = new Mock<ICustomerService>();
        _mockEmployeeService = new Mock<IEmployeeService>();
        var mockPositionService = new Mock<IPositionService>();
        var mockProductCategoryService = new Mock<IProductCategoryService>();
        _mockProductService = new Mock<IProductService>();
        _mockSupplierService = new Mock<ISupplierService>();
        _service = new DataSourceService(
            _mockCustomerService.Object,
            _mockEmployeeService.Object,
            mockPositionService.Object,
            mockProductCategoryService.Object,
            _mockProductService.Object,
            _mockSupplierService.Object);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetCustomersAsync_ReturnsList()
    {
        // Arrange
        _mockCustomerService.Setup(s => s.GetAllForDataSourceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GetAllCustomerForDataSourceResponse(Guid.NewGuid(), "Cust")]);

        // Act
        var result = await _service.GetCustomersAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetEmployeesAsync_ReturnsList()
    {
        // Arrange
        _mockEmployeeService.Setup(s => s.GetAllForDataSourceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GetAllEmployeeForDataSourceResponse(Guid.NewGuid(), "Emp")]);

        // Act
        var result = await _service.GetEmployeesAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsList()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAllForDataSourceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GetAllProductForDataSourceResponse(Guid.NewGuid(), "Prod")]);

        // Act
        var result = await _service.GetProductsAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSuppliersAsync_ReturnsList()
    {
        // Arrange
        _mockSupplierService.Setup(s => s.GetAllForDataSourceAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new GetAllSupplierForDataSourceResponse(Guid.NewGuid(), "Supp")]);

        // Act
        var result = await _service.GetSuppliersAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}
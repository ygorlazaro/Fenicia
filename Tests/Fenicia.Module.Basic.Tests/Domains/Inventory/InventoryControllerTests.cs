using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Inventory;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryControllerTests : IDisposable
{
    private readonly InventoryController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;

    public InventoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new TestCompanyContext());
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        var stockMovementRepository = new StockMovementRepository(_db);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var customerService = new CustomerService(new CustomerRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService), productService);
        var employeeService = new EmployeeService(new EmployeeRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService));
        var supplierService = new SupplierService(new SupplierRepository(_db), productService, stockMovementService, new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)));
        var service = new InventoryService(productService, stockMovementService, orderDetailService, customerService, employeeService, supplierService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new InventoryController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryDashboardAsync(wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryHealthAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryHealthAsync(wide, 90, 3.0, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetInventoryByProductIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.DataSource;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class DataSourceControllerTests : IDisposable
{
    private readonly DataSourceController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;

    public DataSourceControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        var stockMovementRepository = new StockMovementRepository(_db);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var customerService = new CustomerService(new CustomerRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), new OrderService(new OrderRepository(_db), new OrderDetailService(orderDetailRepository), stockMovementService), productService);
        var employeeService = new EmployeeService(new EmployeeRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), new OrderService(new OrderRepository(_db), new OrderDetailService(orderDetailRepository), stockMovementService));
        var positionService = new PositionService(new PositionRepository(_db));
        var productCategoryService = new ProductCategoryService(new ProductCategoryRepository(_db));
        var supplierService = new SupplierService(new SupplierRepository(_db), productService, stockMovementService, new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)));
        var service = new DataSourceService(customerService, employeeService, positionService, productCategoryService, productService, supplierService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new DataSourceController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetPositionsAsync_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetPositionsAsync(wide, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProductCategoriesAsync_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetProductCategoriesAsync(wide, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
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

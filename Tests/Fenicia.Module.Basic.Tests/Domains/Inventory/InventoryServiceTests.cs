using AwesomeAssertions;
using Bogus;
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
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _faker = new Faker();

        var productRepository = new ProductRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var customerRepository = new CustomerRepository(_db);
        var employeeRepository = new EmployeeRepository(_db);
        var supplierRepository = new SupplierRepository(_db);

        var productCategoryService = new ProductCategoryService(new ProductCategoryRepository(_db));
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var productService = new ProductService(productRepository, productCategoryService, orderDetailService, new StockMovementService());
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var addressService = new AddressService(new AddressRepository(_db));
        var personService = new PersonService(new PersonRepository(_db));
        var personAddressService = new PersonAddressService(new PersonAddressRepository(_db));
        var orderService = new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService);
        var customerService = new CustomerService(customerRepository, personService, addressService, personAddressService, orderService, productService);
        var employeeService = new EmployeeService(employeeRepository, personService, addressService, personAddressService, orderService);
        var supplierService = new SupplierService(supplierRepository, productService, stockMovementService, addressService, personAddressService);

        _service = new InventoryService(productService, stockMovementService, orderDetailService, customerService, employeeService, supplierService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetDashboardAsync_WhenDataExists_ReturnsDashboard()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = Guid.NewGuid() };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, Quantity = 10, SalesPrice = _faker.Random.Decimal(), CostPrice = _faker.Random.Decimal() };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetDashboardAsync(new GetInventoryDashboardQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalQuantity.Should().BeGreaterThanOrEqualTo(0);
        result.CategoryBreakdown.Should().NotBeNull();
        result.SupplierBreakdown.Should().NotBeNull();
    }

    [Fact]
    public async Task GetHealthAsync_WhenDataExists_ReturnsHealth()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = Guid.NewGuid() };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, Quantity = 10, SalesPrice = _faker.Random.Decimal() };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetHealthAsync(new GetInventoryHealthQuery(90, 3.0), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }
}

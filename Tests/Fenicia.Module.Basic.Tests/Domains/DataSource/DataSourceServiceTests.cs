using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
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
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class DataSourceServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly CustomerService _customerService;
    private readonly EmployeeService _employeeService;
    private readonly PositionService _positionService;
    private readonly ProductCategoryService _productCategoryService;
    private readonly ProductService _productService;
    private readonly SupplierService _supplierService;
    private readonly Fenicia.Module.Basic.Domains.DataSource.DataSourceService _service;
    private readonly Faker _faker;

    public DataSourceServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new Fenicia.Common.Tests.TestCompanyContext());
        var orderDetailService = new OrderDetailService(new OrderDetailRepository(_db));
        var dummyStockMovementService = new StockMovementService(new Mock<IStockMovementRepository>().Object, new Mock<IProductRepository>().Object);
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), orderDetailService, dummyStockMovementService);
        var stockMovementService = new StockMovementService(new StockMovementRepository(_db), new Mock<IProductRepository>().Object);
        var orderService = new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService);
        _customerService = new CustomerService(new CustomerRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), orderService, productService);
        _employeeService = new EmployeeService(new EmployeeRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), orderService);
        _positionService = new PositionService(new PositionRepository(_db));
        _productCategoryService = new ProductCategoryService(new ProductCategoryRepository(_db));
        _productService = productService;
        _supplierService = new SupplierService(new SupplierRepository(_db), productService, stockMovementService, new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)));
        _service = new Fenicia.Module.Basic.Domains.DataSource.DataSourceService(_customerService, _employeeService, _positionService, _productCategoryService, _productService, _supplierService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetCustomersAsync_WhenCustomersExist_ReturnsListWithNames()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetCustomersAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(customer.Id);
        result.First().Name.Should().Be(customer.Person.Name);
    }

    [Fact]
    public async Task GetEmployeesAsync_WhenEmployeesExist_ReturnsListWithNames()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = position.Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() }, Position = position };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetEmployeesAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(employee.Id);
        result.First().Name.Should().Be(employee.Person.Name);
    }

    [Fact]
    public async Task GetPositionsAsync_WhenPositionsExist_ReturnsListWithNames()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetPositionsAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(position.Id);
        result.First().Name.Should().Be(position.Name);
    }

    [Fact]
    public async Task GetProductCategoriesAsync_WhenCategoriesExist_ReturnsListWithNames()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetProductCategoriesAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(category.Id);
        result.First().Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetProductsAsync_WhenProductsExist_ReturnsListWithNames()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetProductsAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(product.Id);
        result.First().Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetSuppliersAsync_WhenSuppliersExist_ReturnsListWithNames()
    {
        var supplier = new SupplierModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        _db.BasicSuppliers.Add(supplier);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetSuppliersAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(supplier.Id);
        result.First().Name.Should().Be(supplier.Person.Name);
    }
}

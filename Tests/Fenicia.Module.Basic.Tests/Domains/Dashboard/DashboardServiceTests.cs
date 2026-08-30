using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class DashboardServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new Fenicia.Common.Tests.TestCompanyContext());
        var orderRepository = new OrderRepository(_db);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var productRepository = new ProductRepository(_db);
        var productCategoryRepository = new ProductCategoryRepository(_db);
        var supplierRepository = new SupplierRepository(_db);
        var employeeRepository = new EmployeeRepository(_db);
        var personRepository = new PersonRepository(_db);
        var addressRepository = new AddressRepository(_db);
        var personAddressRepository = new PersonAddressRepository(_db);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var dummyStockMovementService = new StockMovementService(new Mock<IStockMovementRepository>().Object, new Mock<IProductRepository>().Object);
        var productService = new ProductService(productRepository, new ProductCategoryService(productCategoryRepository), orderDetailService, dummyStockMovementService);
        var stockMovementService = new StockMovementService(stockMovementRepository, new Mock<IProductRepository>().Object);
        var orderService = new OrderService(orderRepository, new OrderDetailService(orderDetailRepository), new StockMovementService(stockMovementRepository, new Mock<IProductRepository>().Object));
        var employeeService = new EmployeeService(employeeRepository, new PersonService(personRepository), new AddressService(addressRepository), new PersonAddressService(personAddressRepository), orderService);
        _service = new DashboardService(orderService, productService, employeeService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_WhenDataExists_ReturnsDashboardWithAllSections()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = category.Id, Category = category };
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 200, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved };
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = order.Id, ProductId = product.Id, Price = 100, Quantity = 2, Subtotal = 200 };
        order.Details = new List<OrderDetailModel> { orderDetail };

        _db.BasicProducts.Add(product);
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetFinancialDashboardAsync(new GetFinancialDashboardQuery(90), CancellationToken.None);

        result.Should().NotBeNull();
        result.Kpi.Should().NotBeNull();
        result.RevenueVsCost.Should().NotBeEmpty();
        result.ProfitMarginTrend.Should().NotBeEmpty();
        result.AccountsReceivable.Should().NotBeNull();
        result.DailySales.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WhenOrdersExist_ReturnsSumOfTotalAmount()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved };

        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTotalRevenueAsync(CancellationToken.None);

        result.Should().Be(100);
    }

    [Fact]
    public async Task GetTotalCostAsync_WhenOrdersExist_ReturnsSumOfDetailsCost()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = category.Id, Category = category };
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 200, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved };
        var orderDetail = new OrderDetailModel { Id = Guid.NewGuid(), OrderId = order.Id, ProductId = product.Id, Price = 100, Quantity = 2, Subtotal = 200 };
        order.Details = new List<OrderDetailModel> { orderDetail };

        _db.BasicProducts.Add(product);
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTotalCostAsync(CancellationToken.None);

        result.Should().Be(140);
    }

    [Fact]
    public async Task GetTotalOrdersAsync_WhenOrdersExist_ReturnsCount()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved };

        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTotalOrdersAsync(CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetTotalProductsAsync_WhenProductsExist_ReturnsCount()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };

        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTotalProductsAsync(CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetTotalEmployeesAsync_WhenEmployeesExist_ReturnsCount()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = position.Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() }, Position = position };

        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTotalEmployeesAsync(CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task GetRecentOrdersAsync_WhenOrdersExist_ReturnsOrdersOrderedBySaleDate()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order1 = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow.AddDays(-2), Status = OrderStatus.Approved };
        var order2 = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 200, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved };

        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.AddRange(order1, order2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetRecentOrdersAsync(1, CancellationToken.None);

        result.Should().HaveCount(2);
        result.First().Id.Should().Be(order2.Id);
    }

    [Fact]
    public async Task GetTopCustomerOrdersAsync_WhenOrdersExist_ReturnsOrdersWithCustomerAndDetails()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved, Details = new List<OrderDetailModel>() };

        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetTopCustomerOrdersAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Customer.Person.Name.Should().Be(customer.Person.Name);
    }

    [Fact]
    public async Task GetAtRiskOrdersAsync_WhenOrdersExist_ReturnsOrdersWithCustomer()
    {
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow.AddDays(-30), Status = OrderStatus.Approved };

        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAtRiskOrdersAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Customer.Person.Name.Should().Be(customer.Person.Name);
    }

    [Fact]
    public async Task GetEmployeePerformanceOrdersAsync_WhenOrdersExist_ReturnsOrdersWithEmployee()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = position.Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() }, Position = position };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = 100, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved, EmployeeId = employee.Id, Employee = employee };

        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetEmployeePerformanceOrdersAsync(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Employee!.Person.Name.Should().Be(employee.Person.Name);
    }

    [Fact]
    public async Task GetAllEmployeesAsync_WhenEmployeesExist_ReturnsEmployeesWithPersonAndPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel { Id = Guid.NewGuid(), PositionId = position.Id, PersonId = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() }, Position = position };

        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllEmployeesAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Person.Name.Should().Be(employee.Person.Name);
        result.First().Position.Name.Should().Be(position.Name);
    }
}

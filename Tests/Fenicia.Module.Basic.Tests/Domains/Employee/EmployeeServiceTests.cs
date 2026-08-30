using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Employee;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Employee;

public class EmployeeServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new Fenicia.Common.Tests.TestCompanyContext());
        var orderRepository = new OrderRepository(_db);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), orderDetailService, dummyStockMovementService);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var orderService = new OrderService(orderRepository, orderDetailService, stockMovementService);
        _service = new EmployeeService(
            new EmployeeRepository(_db),
            new PersonService(new PersonRepository(_db)),
            new AddressService(new AddressRepository(_db)),
            new PersonAddressService(new PersonAddressRepository(_db)),
            orderService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenEmployeesExist_ReturnsPaginationWithEmployees()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllEmployeeQuery(1, 10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeExists_ReturnsEmployee()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetEmployeeByIdQuery(employee.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(employee.Id);
        result.Name.Should().Be(employee.Person.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetEmployeeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesEmployee()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddEmployeeCommand(Guid.NewGuid(), position.Id, _faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddAsync_WithAddress_CreatesEmployeeWithAddress()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddEmployeeCommand(Guid.NewGuid(), position.Id, _faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), new AddressDTO(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), _faker.Address.Country()));

        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesEmployee()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(employee.Id, position.Id, "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(employee.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        _db.BasicPositions.Add(position);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateEmployeeCommand(Guid.NewGuid(), position.Id, "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeExists_SoftDeletesEmployee()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeleteEmployeeCommand(employee.Id), Guid.NewGuid(), CancellationToken.None);

        var deletedEmployee = await _db.BasicEmployees.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == employee.Id);
        deletedEmployee.Should().NotBeNull();
        deletedEmployee!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenEmployeeDoesNotExist_DoesNothing()
    {
        await _service.DeleteAsync(new DeleteEmployeeCommand(Guid.NewGuid()), Guid.NewGuid(), CancellationToken.None);

        var count = await _db.BasicEmployees.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetByPositionIdAsync_WhenEmployeesExist_ReturnsPaginationWithEmployees()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByPositionIdAsync(new GetEmployeesByPositionIdQuery(position.Id, 1, 10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPerformanceAsync_WhenDataExists_ReturnsPerformanceResponse()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        var customer = new CustomerModel { Id = Guid.NewGuid(), Person = new PersonModel { Name = _faker.Person.FullName, Email = _faker.Internet.Email(), Document = _faker.Person.Random.AlphaNumeric(11), PhoneNumber = _faker.Phone.PhoneNumber() } };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(8), UserId = Guid.NewGuid(), CustomerId = customer.Id, Customer = customer, TotalAmount = 100, SaleDate = DateTime.UtcNow, Status = OrderStatus.Approved, EmployeeId = employee.Id, Employee = employee };

        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetPerformanceAsync(new GetEmployeePerformanceQuery(90, 10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Summary.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllForDataSourceAsync_WhenEmployeesExist_ReturnsListWithNames()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Department() };
        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PositionId = position.Id,
            Position = position,
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicPositions.Add(position);
        _db.BasicEmployees.Add(employee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllForDataSourceAsync(CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(employee.Id);
        result.First().Name.Should().Be(employee.Person.Name);
    }
}

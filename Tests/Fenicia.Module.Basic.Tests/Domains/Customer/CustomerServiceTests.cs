using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Customer;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new Fenicia.Common.Tests.TestCompanyContext());
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryRepository(_db), new SupplierRepository(_db), new OrderDetailRepository(_db), new StockMovementRepository(_db));
        var orderService = new OrderService(new OrderRepository(_db), new OrderDetailService(new OrderDetailRepository(_db)), new StockMovementService(new StockMovementRepository(_db), productService));
        _service = new CustomerService(
            new CustomerRepository(_db),
            new PersonService(new PersonRepository(_db)),
            new AddressService(new AddressRepository(_db)),
            new PersonAddressService(new PersonAddressRepository(_db)),
            orderService,
            productService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ReturnsPaginationWithCustomers()
    {
        var customer = new CustomerModel
        {
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllCustomerQuery(1, 10), CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        var customer = new CustomerModel
        {
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetCustomerByIdQuery(customer.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Name.Should().Be(customer.Person.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetCustomerByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesCustomer()
    {
        var command = new AddCustomerCommand(_faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddAsync_WithAddress_CreatesCustomerWithAddress()
    {
        var command = new AddCustomerCommand(_faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), new AddressCommand(_faker.Address.StreetAddress(), _faker.Address.BuildingNumber(), null, _faker.Address.City(), _faker.Address.ZipCode(), Guid.NewGuid(), _faker.Address.City(), _faker.Address.Country()));

        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesCustomer()
    {
        var customer = new CustomerModel
        {
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(customer.Id, "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var command = new UpdateCustomerCommand(Guid.NewGuid(), "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_SoftDeletesCustomer()
    {
        var customer = new CustomerModel
        {
            Person = new PersonModel
            {
                Name = _faker.Person.FullName,
                Email = _faker.Internet.Email(),
                Document = _faker.Person.Random.AlphaNumeric(11),
                PhoneNumber = _faker.Phone.PhoneNumber()
            }
        };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeleteCustomerCommand(customer.Id), CancellationToken.None);

        var deletedCustomer = await _db.BasicCustomers.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == customer.Id);
        deletedCustomer.Should().NotBeNull();
        deletedCustomer!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_DoesNothing()
    {
        await _service.DeleteAsync(new DeleteCustomerCommand(Guid.NewGuid()), CancellationToken.None);

        var count = await _db.BasicCustomers.CountAsync();
        count.Should().Be(0);
    }
}

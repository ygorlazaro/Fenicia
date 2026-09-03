using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address.DTOs;
using Fenicia.Module.Basic.Domains.Address.Interfaces;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Fenicia.Module.Basic.Domains.PersonAddress.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerServiceTests : IDisposable
{
    private readonly DbContextOptions<DefaultContext> _dbOptions;
    private readonly Faker _faker;
    private readonly Mock<IAddressService> _mockAddressService;
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IPersonAddressService> _mockPersonAddressService;
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _mockRepository = new Mock<ICustomerRepository>();
        var mockPersonService = new Mock<IPersonService>();
        _mockAddressService = new Mock<IAddressService>();
        _mockPersonAddressService = new Mock<IPersonAddressService>();
        _mockOrderService = new Mock<IOrderService>();
        _service = new CustomerService(
            _mockRepository.Object,
            mockPersonService.Object,
            _mockAddressService.Object,
            _mockPersonAddressService.Object,
            _mockOrderService.Object);
        _faker = new Faker();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenCustomersExist_ReturnsPaginationWithCustomers()
    {
        // Arrange
        var db = NewDb();
        var person = new PersonModel
            { Id = Guid.NewGuid(), Name = _faker.Person.FullName, Email = _faker.Internet.Email() };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person };
        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);
        _mockRepository.Setup(r => r.Query()).Returns(() => db.BasicCustomers);

        // Act
        var result = await _service.GetAllAsync(new GetAllCustomerQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        var person = new PersonModel
            { Id = Guid.NewGuid(), Name = _faker.Person.FullName, Email = _faker.Internet.Email() };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _service.GetByIdAsync(new GetCustomerByIdQuery(customer.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(customer.Person.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerModel?)null);

        // Act
        var result = await _service.GetByIdAsync(new GetCustomerByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesCustomer()
    {
        // Arrange
        var command = new AddCustomerCommand(
            _faker.Person.FullName,
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            null);
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<CustomerModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerModel c, CancellationToken _) => c);

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddAsync_WithAddress_CreatesCustomerWithAddress()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var addressResponse = new AddressResponse(
            addressId,
            "Street",
            "100",
            null,
            "Neighborhood",
            "12345-678",
            Guid.NewGuid(),
            "State",
            "City",
            "Country");
        _mockAddressService.Setup(a => a.AddAsync(It.IsAny<AddressCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressResponse);
        _mockPersonAddressService.Setup(p => p.InsertAsync(
                It.IsAny<PersonAddressModel>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PersonAddressModel pa, Guid _, CancellationToken _) => pa);
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<CustomerModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerModel c, CancellationToken _) => c);

        var command = new AddCustomerCommand(
            _faker.Person.FullName,
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            new AddressCommand(
                _faker.Address.StreetAddress(),
                _faker.Address.BuildingNumber(),
                null,
                _faker.Address.City(),
                _faker.Address.ZipCode(),
                Guid.NewGuid(),
                _faker.Address.City(),
                _faker.Address.Country()));

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerExists_UpdatesCustomer()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = "Old Name" };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, Person = person };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(customer.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(r => r.UpdateAsync(customer.Id, It.IsAny<CustomerModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid _, CustomerModel c, CancellationToken _) => c);

        var command = new UpdateCustomerCommand(
            customer.Id,
            "Updated Name",
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            null);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerModel?)null);

        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "Updated Name",
            _faker.Internet.Email(),
            _faker.Person.Random.AlphaNumeric(11),
            _faker.Phone.PhoneNumber(),
            null);

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_DeletesCustomer()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await _service.DeleteAsync(new DeleteCustomerCommand(customerId), Guid.NewGuid(), CancellationToken.None);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetInsightsAsync_WhenCalled_ReturnsInsights()
    {
        // Arrange
        _mockOrderService.Setup(o => o.GetTotalOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _mockOrderService.Setup(o => o.GetTotalRevenueAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1000m);
        _mockOrderService.Setup(o => o.GetRecentOrdersAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);
        _mockOrderService.Setup(o => o.GetTopCustomerOrdersAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockOrderService.Setup(o => o.GetAtRiskOrdersAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);

        // Act
        var result = await _service.GetInsightsAsync(new GetCustomerInsightsQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCountAsync_ReturnsCount()
    {
        // Arrange
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(7);

        // Act
        var result = await _service.GetCountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(7);
    }

    private DefaultContext NewDb()
    {
        return new DefaultContext(_dbOptions, new TestCompanyContext());
    }
}
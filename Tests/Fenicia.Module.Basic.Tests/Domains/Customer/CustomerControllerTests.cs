using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Tests.Domains.Customer;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class CustomerControllerTests : IDisposable
{
    private readonly CustomerController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;

    public CustomerControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        var stockMovementRepository = new StockMovementRepository(_db);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        var orderService = new OrderService(new OrderRepository(_db), new OrderDetailService(orderDetailRepository), stockMovementService);
        var service = new CustomerService(new CustomerRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), orderService, productService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new CustomerController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddCustomerCommand(_faker.Person.FullName, _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerExists_ReturnsOk()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName(), CompanyId = _companyContext.CompanyId };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(customer.Id, "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, customer.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateCustomerCommand(Guid.NewGuid(), "Updated Name", _faker.Internet.Email(), _faker.Person.Random.AlphaNumeric(11), _faker.Phone.PhoneNumber(), null);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_ReturnsNoContent()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName(), CompanyId = _companyContext.CompanyId };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(customer.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenCustomersExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerExists_ReturnsOk()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Name.FullName(), CompanyId = _companyContext.CompanyId };
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(customer.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCustomerDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
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

using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Tests.Domains.Order;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderControllerTests : IDisposable
{
    private readonly OrderController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;
    private readonly OrderService _service;

    public OrderControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var stockMovementRepository = new StockMovementRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), orderDetailService, dummyStockMovementService);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        _service = new OrderService(new OrderRepository(_db), orderDetailService, stockMovementService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new OrderController(_service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicCustomers.Add(customer);
        await _db.SaveChangesAsync(CancellationToken.None);

        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = _companyContext.CompanyId };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, CompanyId = _companyContext.CompanyId, SalesPrice = 100, Quantity = 10 };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(Guid.NewGuid(), customer.Id, DateTime.UtcNow, OrderStatus.Pending, [new OrderDetailCommand(product.Id, 100, 2)], PaymentMethod.Cash);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_ReturnsNoContent()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), UserId = Guid.NewGuid(), CustomerId = customer.Id, TotalAmount = _faker.Random.Decimal(), DiscountAmount = _faker.Random.Decimal(), TotalQuantity = _faker.Random.Int(), SaleDate = _faker.Date.Recent(), Status = OrderStatus.Pending, PaymentMethod = PaymentMethod.Cash, CompanyId = _companyContext.CompanyId };
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(order.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenOrdersExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOk()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), UserId = Guid.NewGuid(), CustomerId = customer.Id, TotalAmount = _faker.Random.Decimal(), DiscountAmount = _faker.Random.Decimal(), TotalQuantity = _faker.Random.Int(), SaleDate = _faker.Date.Recent(), Status = OrderStatus.Pending, PaymentMethod = PaymentMethod.Cash, CompanyId = _companyContext.CompanyId };
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(order.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNotFound()
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

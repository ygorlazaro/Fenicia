using System.Security.Claims;

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
    public async Task GetAsync_WhenOrdersExist_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOk()
    {
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName, CompanyId = _companyContext.CompanyId };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id, CompanyId = _companyContext.CompanyId };
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), UserId = Guid.NewGuid(), CustomerId = customer.Id, TotalAmount = _faker.Random.Decimal(), DiscountAmount = _faker.Random.Decimal(), TotalQuantity = _faker.Random.Int(), SaleDate = _faker.Date.Recent(), Status = OrderStatus.Pending, PaymentMethod = PaymentMethod.Cash, CompanyId = _companyContext.CompanyId };
        _db.BasicCustomers.Add(customer);
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var repo = new OrderRepository(_db);
        var found = await repo.GetByIdWithDetailsAsync(order.Id, CancellationToken.None);
        Assert.NotNull(found);

        var serviceResult = await _service.GetByIdAsync(new GetOrderByIdQuery(order.Id), CancellationToken.None);
        Assert.NotNull(serviceResult);

        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(order.Id, wide, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);
        Assert.IsType<NotFoundResult>(result.Result);
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

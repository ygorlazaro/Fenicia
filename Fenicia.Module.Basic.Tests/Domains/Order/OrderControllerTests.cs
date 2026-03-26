using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.Commands;
using Fenicia.Module.Basic.Domains.Order.Handlers;
using Fenicia.Module.Basic.Domains.Order.Responses;
using Fenicia.Module.Basic.Domains.OrderDetail.Handlers;
using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

/// <summary>
///     Unit tests for the OrderController.
///     Tests HTTP endpoints for order management including CRUD operations and analytics.
/// </summary>
public class OrderControllerTests : IDisposable
{
    private readonly OrderController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testCustomerId;
    private readonly Guid testOrderId;
    private readonly Guid testUserId;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testOrderId = Guid.NewGuid();
        testUserId = Guid.NewGuid();
        testCustomerId = Guid.NewGuid();
        var createOrderHandler = new CreateOrderHandler(db);
        var getOrderDetailsByOrderIdHandler = new GetOrderDetailsByOrderIdHandler(db);
        var getAllOrderHandler = new GetAllOrderHandler(db);
        var getOrderByIdHandler = new GetOrderByIdHandler(db);
        var deleteOrderHandler = new DeleteOrderHandler(db);
        var getOrderAnalyticsHandler = new GetOrderAnalyticsHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new OrderController(getAllOrderHandler, getOrderByIdHandler, createOrderHandler, deleteOrderHandler, getOrderDetailsByOrderIdHandler, getOrderAnalyticsHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidCommand_ReturnsCreatedWithOrder()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        db.BasicCustomers.Add(customer);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(testUserId, testCustomerId, DateTime.Now, OrderStatus.Pending, [new OrderDetailCommand(product.Id, 20.00m, 2), new OrderDetailCommand(product.Id, 20.00m, 3)], PaymentMethod.CreditCard);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedOrder = createdResult.Value as CreateOrderResponse;
        Assert.NotNull(returnedOrder);
        Assert.Equal(testCustomerId, returnedOrder.CustomerId);
        Assert.Equal(testUserId, returnedOrder.UserId);
        Assert.True(returnedOrder.TotalAmount > 0);
    }

    [Fact]
    public async Task GetDetailsAsync_WhenOrderExists_ReturnsOkWithOrderDetails()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = testOrderId,
            OrderNumber = "ORD-20260101-TEST001",
            UserId = testUserId,
            CustomerId = testCustomerId,
            SaleDate = DateTime.Now,
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m,
            TotalQuantity = 5,
            PaymentMethod = PaymentMethod.CreditCard
        };

        var orderDetail1 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = testOrderId,
            ProductId = Guid.NewGuid(),
            Price = 20.00m,
            Quantity = 2,
            DiscountAmount = 0,
            Subtotal = 40.00m
        };

        var orderDetail2 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = testOrderId,
            ProductId = Guid.NewGuid(),
            Price = 30.00m,
            Quantity = 3,
            DiscountAmount = 0,
            Subtotal = 90.00m
        };

        db.BasicOrders.Add(order);
        db.BasicOrderDetails.AddRange(orderDetail1, orderDetail2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetDetailsAsync(testOrderId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDetails = okResult.Value as List<GetOrderDetailsByOrderIdResponse>;
        Assert.NotNull(returnedDetails);
        Assert.Equal(2, returnedDetails.Count);
    }

    [Fact]
    public async Task GetDetailsAsync_WhenOrderDoesNotExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetDetailsAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDetails = okResult.Value as List<GetOrderDetailsByOrderIdResponse>;
        Assert.NotNull(returnedDetails);
        Assert.Empty(returnedDetails);
    }

    [Fact]
    public async Task CreateOrderAsync_SetsUserIdFromClaims()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        db.BasicCustomers.Add(customer);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(Guid.Empty, // Will be overridden by claims
            testCustomerId, DateTime.Now, OrderStatus.Pending, [new OrderDetailCommand(product.Id, 20.00m, 2)], PaymentMethod.CreditCard);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);

        var returnedOrder = createdResult.Value as CreateOrderResponse;
        Assert.NotNull(returnedOrder);
        Assert.Equal(testUserId, returnedOrder.UserId);
    }

    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void OrderController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void OrderController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;
using Fenicia.Module.Basic.Domains.Order.Delete;
using Fenicia.Module.Basic.Domains.Order.GetAll;
using Fenicia.Module.Basic.Domains.Order.GetById;
using Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderControllerTests : IDisposable
{
    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testOrderId = Guid.NewGuid();
        this.testUserId = Guid.NewGuid();
        this.testCustomerId = Guid.NewGuid();
        var createOrderHandler = new CreateOrderHandler(this.context);
        var getOrderDetailsByOrderIdHandler = new GetOrderDetailsByOrderIdHandler(this.context);
        var getAllOrderHandler = new GetAllOrderHandler(this.context);
        var getOrderByIdHandler = new GetOrderByIdHandler(this.context);
        var deleteOrderHandler = new DeleteOrderHandler(this.context);
        var getOrderAnalyticsHandler = new GetOrderAnalyticsHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new OrderController(
            getAllOrderHandler,
            getOrderByIdHandler,
            createOrderHandler,
            deleteOrderHandler,
            getOrderDetailsByOrderIdHandler,
            getOrderAnalyticsHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    private readonly OrderController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testOrderId;
    private readonly Guid testUserId;
    private readonly Guid testCustomerId;
    private readonly Faker faker;

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidCommand_ReturnsCreatedWithOrder()
    {
        // Arrange
        var customer = new CustomerModel
        {
            Id = this.testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        this.context.BasicCustomers.Add(customer);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(
            this.testUserId,
            this.testCustomerId,
            DateTime.Now,
            OrderStatus.Pending,
            [
                new OrderDetailCommand(product.Id, 20.00m, 2),
                new OrderDetailCommand(product.Id, 20.00m, 3)
            ]);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedOrder = createdResult.Value as CreateOrderResponse;
        Assert.NotNull(returnedOrder);
        Assert.Equal(this.testCustomerId, returnedOrder.CustomerId);
        Assert.Equal(this.testUserId, returnedOrder.UserId);
        Assert.True(returnedOrder.TotalAmount > 0);
    }

    [Fact]
    public async Task GetDetailsAsync_WhenOrderExists_ReturnsOkWithOrderDetails()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = this.testOrderId,
            UserId = this.testUserId,
            CustomerId = this.testCustomerId,
            SaleDate = DateTime.Now,
            Status = OrderStatus.Pending,
            TotalAmount = 100.00m
        };

        var orderDetail1 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = this.testOrderId,
            ProductId = Guid.NewGuid(),
            Price = 20.00m,
            Quantity = 2
        };

        var orderDetail2 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = this.testOrderId,
            ProductId = Guid.NewGuid(),
            Price = 30.00m,
            Quantity = 3
        };

        this.context.BasicOrders.Add(order);
        this.context.BasicOrderDetails.AddRange(orderDetail1, orderDetail2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetDetailsAsync(this.testOrderId, wide, ct);

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
        var result = await this.controller.GetDetailsAsync(nonExistentId, wide, ct);

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
            Id = this.testCustomerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##")
            }
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        this.context.BasicCustomers.Add(customer);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(
            Guid.Empty, // Will be overridden by claims
            this.testCustomerId,
            DateTime.Now,
            OrderStatus.Pending,
            [new OrderDetailCommand(product.Id, 20.00m, 2)]);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);

        var returnedOrder = createdResult.Value as CreateOrderResponse;
        Assert.NotNull(returnedOrder);
        Assert.Equal(this.testUserId, returnedOrder.UserId);
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
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

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
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    public void Dispose()
    {
        this.context.Dispose();
    }
}

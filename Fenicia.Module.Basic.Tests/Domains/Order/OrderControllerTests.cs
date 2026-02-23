using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

[TestFixture]
public class OrderControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testOrderId = Guid.NewGuid();
        this.testUserId = Guid.NewGuid();
        this.testCustomerId = Guid.NewGuid();
        this.createOrderHandler = new CreateOrderHandler(this.context);
        this.getOrderDetailsByOrderIdHandler = new GetOrderDetailsByOrderIdHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new OrderController(
            this.createOrderHandler,
            this.getOrderDetailsByOrderIdHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims(this.testUserId);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private OrderController controller = null!;
    private BasicContext context = null!;
    private CreateOrderHandler createOrderHandler = null!;
    private GetOrderDetailsByOrderIdHandler getOrderDetailsByOrderIdHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testOrderId;
    private Guid testUserId;
    private Guid testCustomerId;
    private Faker faker = null!;

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

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##")
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

        this.context.Customers.Add(customer);
        this.context.Products.Add(product);
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.CreateOrderAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedOrder = createdResult.Value as OrderResponse;
        Assert.That(returnedOrder, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedOrder.CustomerId, Is.EqualTo(this.testCustomerId));
            Assert.That(returnedOrder.UserId, Is.EqualTo(this.testUserId));
            Assert.That(returnedOrder.TotalAmount, Is.GreaterThan(0));
        }
    }

    [Test]
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

        this.context.Orders.Add(order);
        this.context.OrderDetails.AddRange(orderDetail1, orderDetail2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetDetailsAsync(this.testOrderId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDetails = okResult.Value as List<OrderDetailResponse>;
        Assert.That(returnedDetails, Is.Not.Null);
        Assert.That(returnedDetails, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetDetailsAsync_WhenOrderDoesNotExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetDetailsAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDetails = okResult.Value as List<OrderDetailResponse>;
        Assert.That(returnedDetails, Is.Not.Null);
        Assert.That(returnedDetails, Is.Empty);
    }

    [Test]
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
                Cpf = this.faker.Random.Replace("###.###.###-##")
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

        this.context.Customers.Add(customer);
        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(
            Guid.Empty, // Will be overridden by claims
            this.testCustomerId,
            DateTime.Now,
            OrderStatus.Pending,
            [new OrderDetailCommand(product.Id, 20.00m, 2)]);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.CreateOrderAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);

        var returnedOrder = createdResult.Value as OrderResponse;
        Assert.That(returnedOrder, Is.Not.Null);
        Assert.That(returnedOrder.UserId, Is.EqualTo(this.testUserId));
    }

    [Test]
    public void OrderController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "OrderController should have Authorize attribute");
    }

    [Test]
    public void OrderController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "OrderController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void OrderController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(OrderController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "OrderController should have ApiController attribute");
    }
}

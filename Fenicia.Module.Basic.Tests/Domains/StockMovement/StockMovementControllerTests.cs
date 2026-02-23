using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Add;
using Fenicia.Module.Basic.Domains.StockMovement.GetMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

[TestFixture]
public class StockMovementControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testMovementId = Guid.NewGuid();
        this.testProductId = Guid.NewGuid();
        this.getStockMovementHandler = new GetStockMovementHandler(this.context);
        this.addStockMovementHandler = new AddStockMovementHandler(this.context);
        this.updateStockMovementHandler = new UpdateStockMovementHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new StockMovementController(
            this.getStockMovementHandler,
            this.addStockMovementHandler,
            this.updateStockMovementHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private StockMovementController controller = null!;
    private BasicContext context = null!;
    private GetStockMovementHandler getStockMovementHandler = null!;
    private AddStockMovementHandler addStockMovementHandler = null!;
    private UpdateStockMovementHandler updateStockMovementHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testMovementId;
    private Guid testProductId;
    private Faker faker = null!;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId", Guid.NewGuid().ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Test]
    public async Task GetAsync_WhenNoMovementsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var query = new StockMovementController.StockMovementQuery(1, 10)
        {
            StartDate = DateTime.Now.AddDays(-30),
            EndDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedMovements = okResult.Value as List<StockMovementResponse>;
        Assert.That(returnedMovements, Is.Not.Null);
        Assert.That(returnedMovements, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenMovementsExist_ReturnsOkWithMovements()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var movement1 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        };

        var movement2 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 5,
            Date = DateTime.Now.AddDays(-3),
            Price = 20.00m,
            Type = StockMovementType.Out
        };

        this.context.Products.Add(product);
        this.context.StockMovements.AddRange(movement1, movement2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new StockMovementController.StockMovementQuery(1, 10)
        {
            StartDate = DateTime.Now.AddDays(-30),
            EndDate = DateTime.Now
        };
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedMovements = okResult.Value as List<StockMovementResponse>;
        Assert.That(returnedMovements, Is.Not.Null);
        Assert.That(returnedMovements, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithMovement()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            20.00m,
            StockMovementType.In,
            this.testProductId,
            null,
            null);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedMovement = createdResult.Value as StockMovementResponse;
        Assert.That(returnedMovement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedMovement.ProductId, Is.EqualTo(this.testProductId));
            Assert.That(returnedMovement.Quantity, Is.EqualTo(10));
            Assert.That(returnedMovement.Type, Is.EqualTo(StockMovementType.In));
        }
    }

    [Test]
    public async Task PatchAsync_WhenMovementExists_ReturnsCreatedWithUpdatedMovement()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var movement = new StockMovementModel
        {
            Id = this.testMovementId,
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        };

        this.context.Products.Add(product);
        this.context.StockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        SetupAdminUserClaims();

        var command = new UpdateStockMovementCommand(
            this.testMovementId,
            15,
            DateTime.Now,
            25.00m,
            StockMovementType.In,
            this.testProductId,
            null,
            null);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(this.testMovementId, command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);

        var returnedMovement = createdResult.Value as StockMovementResponse;
        Assert.That(returnedMovement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedMovement.Quantity, Is.EqualTo(15));
            Assert.That(returnedMovement.Price, Is.EqualTo(25.00m));
        }
    }

    [Test]
    public async Task PatchAsync_WhenMovementDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        SetupAdminUserClaims();

        var command = new UpdateStockMovementCommand(
            nonExistentId,
            15,
            DateTime.Now,
            25.00m,
            StockMovementType.In,
            this.testProductId,
            null,
            null);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(nonExistentId, command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public void StockMovementController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "StockMovementController should have Authorize attribute");
    }

    [Test]
    public void StockMovementController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "StockMovementController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void StockMovementController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "StockMovementController should have ApiController attribute");
    }

    [Test]
    public void PatchAsync_HasAuthorizeRolesAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);
        var methodInfo = controllerType.GetMethod(nameof(StockMovementController.PatchAsync));

        // Act
        var authorizeAttribute =
            methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    private void SetupAdminUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId", Guid.NewGuid().ToString()),
            new("role", "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

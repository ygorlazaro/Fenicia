using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Add;
using Fenicia.Module.Basic.Domains.StockMovement.GetMovement;
using Fenicia.Module.Basic.Domains.StockMovement.GetStockMovementDashboard;
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
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testMovementId = Guid.NewGuid();
        this.testProductId = Guid.NewGuid();
        this.getStockMovementHandler = new GetStockMovementHandler(this.context);
        this.addStockMovementHandler = new AddStockMovementHandler(this.context);
        this.updateStockMovementHandler = new UpdateStockMovementHandler(this.context);
        this.getStockMovementDashboardHandler = new GetStockMovementDashboardHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new StockMovementController(
            this.getStockMovementHandler,
            this.addStockMovementHandler,
            this.updateStockMovementHandler,
            this.getStockMovementDashboardHandler)
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

    private TestCompanyContext companyContext = null!;
    private StockMovementController controller = null!;
    private DefaultContext context = null!;
    private GetStockMovementHandler getStockMovementHandler = null!;
    private AddStockMovementHandler addStockMovementHandler = null!;
    private UpdateStockMovementHandler updateStockMovementHandler = null!;
    private GetStockMovementDashboardHandler getStockMovementDashboardHandler = null!;
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

        var returnedMovements = okResult.Value as List<GetStockMovementResponse>;
        Assert.That(returnedMovements, Is.Not.Null);
        Assert.That(returnedMovements, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenMovementsExist_ReturnsOkWithMovements()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var movement1 = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        };

        var movement2 = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 5,
            Date = DateTime.Now.AddDays(-3),
            Price = 20.00m,
            Type = StockMovementType.Out
        };

        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.AddRange(movement1, movement2);
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

        var returnedMovements = okResult.Value as List<GetStockMovementResponse>;
        Assert.That(returnedMovements, Is.Not.Null);
        Assert.That(returnedMovements, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithMovement()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            20.00m,
            StockMovementType.In,
            this.testProductId,
            null,
            null,
            "Test reason");

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedMovement = createdResult.Value as AddStockMovementResponse;
        Assert.That(returnedMovement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedMovement.ProductId, Is.EqualTo(this.testProductId));
            Assert.That(returnedMovement.Quantity, Is.EqualTo(10));
            Assert.That(returnedMovement.Type, Is.EqualTo(StockMovementType.In));
            Assert.That(returnedMovement.Reason, Is.EqualTo("Test reason"));
        }
    }

    [Test]
    public async Task PatchAsync_WhenMovementExists_ReturnsCreatedWithUpdatedMovement()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var movement = new BasicStockMovementModel
        {
            Id = this.testMovementId,
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In
        };

        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.Add(movement);
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
            null,
            "Updated reason");

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(this.testMovementId, command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);

        var returnedMovement = createdResult.Value as UpdateStockMovementResponse;
        Assert.That(returnedMovement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedMovement.Quantity, Is.EqualTo(15));
            Assert.That(returnedMovement.Price, Is.EqualTo(25.00m));
            Assert.That(returnedMovement.Reason, Is.EqualTo("Updated reason"));
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

    [Test]
    public async Task GetDashboardAsync_WithNoMovements_ReturnsEmptyDashboard()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetDashboardAsync(30, 10, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var dashboard = okResult.Value as StockMovementDashboardResponse;
        Assert.That(dashboard, Is.Not.Null);
        Assert.That(dashboard.History, Is.Empty);
        Assert.That(dashboard.MonthlyInOut, Is.Empty);
        Assert.That(dashboard.TopMovedProducts, Is.Empty);
        Assert.That(dashboard.TurnoverRates, Is.Empty);
    }

    [Test]
    public async Task GetDashboardAsync_WithMovements_ReturnsDashboardData()
    {
        // Arrange
        var product = new BasicProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var category = new BasicProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var movement = new BasicStockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In,
            Reason = "Test reason"
        };

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        this.context.BasicStockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetDashboardAsync(30, 10, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var dashboard = okResult.Value as StockMovementDashboardResponse;
        Assert.That(dashboard, Is.Not.Null);
        Assert.That(dashboard.History, Is.Not.Empty);
        Assert.That(dashboard.History[0].ProductName, Is.EqualTo(product.Name));
        Assert.That(dashboard.History[0].Reason, Is.EqualTo("Test reason"));
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

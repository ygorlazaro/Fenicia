using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementControllerTests : IDisposable
{
    public StockMovementControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            this.companyContext);
        this.testMovementId = Guid.NewGuid();
        this.testProductId = Guid.NewGuid();
        this.getStockMovementHandler = new GetStockMovementHandler(this.db);
        this.addStockMovementHandler = new AddStockMovementHandler(this.db);
        this.updateStockMovementHandler = new UpdateStockMovementHandler(this.db);
        this.getStockMovementDashboardHandler = new GetStockMovementDashboardHandler(this.db);
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

    private readonly TestCompanyContext companyContext;
    private readonly StockMovementController controller;
    private readonly DefaultContext db;
    private readonly GetStockMovementHandler getStockMovementHandler;
    private readonly AddStockMovementHandler addStockMovementHandler;
    private readonly UpdateStockMovementHandler updateStockMovementHandler;
    private readonly GetStockMovementDashboardHandler getStockMovementDashboardHandler;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testMovementId;
    private readonly Guid testProductId;
    private readonly Faker faker;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId",
                Guid.NewGuid()
                    .ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenNoMovementsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var query = new StockMovementController.StockMovementQuery(1,
            10)
        {
            StartDate = DateTime.Now.AddDays(-30),
            EndDate = DateTime.Now
        };
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedMovements = okResult.Value as List<GetStockMovementResponse>;
        Assert.NotNull(returnedMovements);
        Assert.Empty(returnedMovements);
    }

    [Fact]
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

        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.AddRange(movement1,
            movement2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new StockMovementController.StockMovementQuery(1,
            10)
        {
            StartDate = DateTime.Now.AddDays(-30),
            EndDate = DateTime.Now
        };
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedMovements = okResult.Value as List<GetStockMovementResponse>;
        Assert.NotNull(returnedMovements);
        Assert.Equal(2,
            returnedMovements.Count);
    }

    [Fact]
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

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            20.00m,
            StockMovementType.In,
            this.testProductId,
            null,
            null,
            null,
            null,
            "Test reason");

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201,
            createdResult.StatusCode);

        var returnedMovement = createdResult.Value as AddStockMovementResponse;
        Assert.NotNull(returnedMovement);
        Assert.Equal(this.testProductId,
            returnedMovement.ProductId);
        Assert.Equal(10,
            returnedMovement.Quantity);
        Assert.Equal(StockMovementType.In,
            returnedMovement.Type);
        Assert.Equal("Test reason",
            returnedMovement.Reason);
    }

    [Fact]
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

        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.Add(movement);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
            null,
            null,
            "Updated reason");

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(this.testMovementId,
            command,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);

        var returnedMovement = createdResult.Value as UpdateStockMovementResponse;
        Assert.NotNull(returnedMovement);
        Assert.Equal(15,
            returnedMovement.Quantity);
        Assert.Equal(25.00m,
            returnedMovement.Price);
        Assert.Equal("Updated reason",
            returnedMovement.Reason);
    }

    [Fact]
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
            null,
            null,
            null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(nonExistentId,
            command,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void StockMovementController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void StockMovementController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute),
                false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]",
            routeAttribute.Template);
    }

    [Fact]
    public void StockMovementController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute),
                false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void PatchAsync_HasAuthorizeRolesAttribute()
    {
        // Arrange
        var controllerType = typeof(StockMovementController);
        var methodInfo = controllerType.GetMethod(nameof(StockMovementController.PatchAsync));

        // Act
        var authorizeAttribute =
            methodInfo?.GetCustomAttributes(typeof(AuthorizeAttribute),
                false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin",
            authorizeAttribute.Roles);
    }

    [Fact]
    public async Task GetDashboardAsync_WithNoMovements_ReturnsEmptyDashboard()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetDashboardAsync(wide,
            30,
            10,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var dashboard = okResult.Value as StockMovementDashboardResponse;
        Assert.NotNull(dashboard);
        Assert.Empty(dashboard.History);
        Assert.Empty(dashboard.MonthlyInOut);
        Assert.Empty(dashboard.TopMovedProducts);
        Assert.Empty(dashboard.TurnoverRates);
    }

    [Fact]
    public async Task GetDashboardAsync_WithMovements_ReturnsDashboardData()
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

        var category = new ProductCategoryModel
        {
            Id = product.CategoryId,
            Name = "Test Category"
        };

        var movement = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = this.testProductId,
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 20.00m,
            Type = StockMovementType.In,
            Reason = "Test reason"
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        this.db.BasicStockMovements.Add(movement);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetDashboardAsync(wide,
            30,
            10,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var dashboard = okResult.Value as StockMovementDashboardResponse;
        Assert.NotNull(dashboard);
        Assert.NotEmpty(dashboard.History);
        Assert.Equal(product.Name,
            dashboard.History[0].ProductName);
        Assert.Equal("Test reason",
            dashboard.History[0].Reason);
    }

    private void SetupAdminUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId",
                Guid.NewGuid()
                    .ToString()),
            new("role",
                "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

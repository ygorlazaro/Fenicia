using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryControllerTests : IDisposable
{
    public InventoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.testProductId = Guid.NewGuid();
        this.testCategoryId = Guid.NewGuid();
        var getInventoryHandler = new GetInventoryHandler(this.db);
        var getInventoryByProductHandler = new GetInventoryByProductHandler(this.db);
        var getInventoryByCategoryHandler = new GetInventoryByCategoryHandler(this.db);
        var getInventoryDashboardHandler = new GetInventoryDashboardHandler(this.db);
        var getInventoryHealthHandler = new GetInventoryHealthHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new InventoryController(
            getInventoryHandler,
            getInventoryByProductHandler,
            getInventoryByCategoryHandler,
            getInventoryDashboardHandler,
            getInventoryHealthHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    private readonly InventoryController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProductId;
    private readonly Guid testCategoryId;
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
    public async Task GetInventoryAsync_WhenNoProductsExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page,
            perPage,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Empty(returnedInventory.Items);
        Assert.Equal(0,
            returnedInventory.TotalCostPrice);
        Assert.Equal(0,
            returnedInventory.TotalSalesPrice);
        Assert.Equal(0,
            returnedInventory.TotalQuantity);
    }

    [Fact]
    public async Task GetInventoryAsync_WhenProductsExist_ReturnsOkWithInventory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.AddRange(product1,
            product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page,
            perPage,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Equal(2,
            returnedInventory.Items.Count);
        Assert.Equal(25.00m,
            returnedInventory.TotalCostPrice);
        Assert.Equal(50.00m,
            returnedInventory.TotalSalesPrice);
        Assert.Equal(150,
            returnedInventory.TotalQuantity);
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_WhenProductExists_ReturnsOkWithInventory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = this.testProductId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(this.testProductId,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Single(returnedInventory.Items);
        Assert.Equal(this.testProductId,
            returnedInventory.Items[0].Id);
        Assert.Equal(product.Name,
            returnedInventory.Items[0].Name);
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_WhenProductDoesNotExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(nonExistentId,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Empty(returnedInventory.Items);
    }

    [Fact]
    public async Task GetInventoryByCategoryIdAsync_WhenCategoryExists_ReturnsOkWithInventory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.AddRange(product1,
            product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(this.testCategoryId,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Equal(2,
            returnedInventory.Items.Count);
        Assert.Equal(category.Id,
            returnedInventory.Items[0].CategoryId);
        Assert.Equal(category.Id,
            returnedInventory.Items[1].CategoryId);
    }

    [Fact]
    public async Task GetInventoryByCategoryIdAsync_WhenCategoryDoesNotExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(nonExistentId,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Empty(returnedInventory.Items);
    }

    #region GetInventoryDashboardAsync Tests

    [Fact]
    public async Task GetInventoryDashboardAsync_WhenNoDataExists_ReturnsOkWithEmptyDashboard()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.Empty(returnedDashboard.LowStockItems);
        Assert.Equal(0,
            returnedDashboard.TotalCustomers);
        Assert.Equal(0,
            returnedDashboard.TotalEmployees);
        Assert.Equal(0,
            returnedDashboard.TotalCostValue);
        Assert.Equal(0,
            returnedDashboard.TotalSalesValue);
        Assert.Equal(0,
            returnedDashboard.TotalQuantity);
        Assert.Equal(0,
            returnedDashboard.ProfitPotential);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_WhenProductsExist_ReturnsOkWithDashboardData()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 5,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Name.FullName()
            }
        };

        var employee = new EmployeeModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            PositionId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Name.FullName()
            }
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.AddRange(product1,
            product2);
        this.db.BasicCustomers.Add(customer);
        this.db.BasicEmployees.Add(employee);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.NotEmpty(returnedDashboard.LowStockItems);
        Assert.Equal(1,
            returnedDashboard.TotalCustomers);
        Assert.Equal(1,
            returnedDashboard.TotalEmployees);
        Assert.Equal(105,
            returnedDashboard.TotalQuantity);
        Assert.Single(returnedDashboard.CategoryBreakdown);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_ReturnsLowStockItemsOrderedByQuantity()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Low Stock Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 2,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "High Stock Product",
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 200,
            CategoryId = category.Id
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.AddRange(product1,
            product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.NotEmpty(returnedDashboard.LowStockItems);

        // First item should be the one with the lowest quantity
        Assert.Equal("Low Stock Product",
            returnedDashboard.LowStockItems[0].Name);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_CalculatesProfitPotentialCorrectly()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 10,
            CategoryId = category.Id
        };

        this.db.BasicProductCategories.Add(category);
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);

        // Total Cost Value = 10.00 * 10 = 100.00
        // Total Sales Value = 20.00 * 10 = 200.00
        // Profit Potential = 200.00 - 100.00 = 100.00
        Assert.Equal(100.00m,
            returnedDashboard.TotalCostValue);
        Assert.Equal(200.00m,
            returnedDashboard.TotalSalesValue);
        Assert.Equal(100.00m,
            returnedDashboard.ProfitPotential);
    }

    #endregion

    [Fact]
    public void InventoryController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void InventoryController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

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
    public void InventoryController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute),
                false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }
}

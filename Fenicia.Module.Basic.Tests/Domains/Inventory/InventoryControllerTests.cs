using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryDashboard;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryHealth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

[TestFixture]
public class InventoryControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProductId = Guid.NewGuid();
        this.testCategoryId = Guid.NewGuid();
        this.getInventoryHandler = new GetInventoryHandler(this.context);
        this.getInventoryByProductHandler = new GetInventoryByProductHandler(this.context);
        this.getInventoryByCategoryHandler = new GetInventoryByCategoryHandler(this.context);
        this.getInventoryDashboardHandler = new GetInventoryDashboardHandler(this.context);
        this.getInventoryHealthHandler = new GetInventoryHealthHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new InventoryController(
            this.getInventoryHandler,
            this.getInventoryByProductHandler,
            this.getInventoryByCategoryHandler,
            this.getInventoryDashboardHandler,
            this.getInventoryHealthHandler)
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
    private InventoryController controller = null!;
    private DefaultContext context = null!;
    private GetInventoryHandler getInventoryHandler = null!;
    private GetInventoryByProductHandler getInventoryByProductHandler = null!;
    private GetInventoryByCategoryHandler getInventoryByCategoryHandler = null!;
    private GetInventoryDashboardHandler getInventoryDashboardHandler = null!;
    private GetInventoryHealthHandler getInventoryHealthHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProductId;
    private Guid testCategoryId;
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
    public async Task GetInventoryAsync_WhenNoProductsExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page, perPage, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedInventory.Items, Is.Empty);
            Assert.That(returnedInventory.TotalCostPrice, Is.EqualTo(0));
            Assert.That(returnedInventory.TotalSalesPrice, Is.EqualTo(0));
            Assert.That(returnedInventory.TotalQuantity, Is.EqualTo(0));
        }
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page, perPage, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedInventory.Items, Has.Count.EqualTo(2));
            Assert.That(returnedInventory.TotalCostPrice, Is.EqualTo(25.00m));
            Assert.That(returnedInventory.TotalSalesPrice, Is.EqualTo(50.00m));
            Assert.That(returnedInventory.TotalQuantity, Is.EqualTo(150));
        }
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(this.testProductId, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedInventory.Items, Has.Count.EqualTo(1));
            Assert.That(returnedInventory.Items[0].Id, Is.EqualTo(this.testProductId));
            Assert.That(returnedInventory.Items[0].Name, Is.EqualTo(product.Name));
        }
    }

    [Test]
    public async Task GetInventoryByProductIdAsync_WhenProductDoesNotExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(nonExistentId, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        Assert.That(returnedInventory.Items, Is.Empty);
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(this.testCategoryId, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedInventory.Items, Has.Count.EqualTo(2));
            Assert.That(returnedInventory.Items[0].CategoryId, Is.EqualTo(category.Id));
            Assert.That(returnedInventory.Items[1].CategoryId, Is.EqualTo(category.Id));
        }
    }

    [Test]
    public async Task GetInventoryByCategoryIdAsync_WhenCategoryDoesNotExist_ReturnsOkWithEmptyInventory()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(nonExistentId, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.That(returnedInventory, Is.Not.Null);
        Assert.That(returnedInventory.Items, Is.Empty);
    }

    #region GetInventoryDashboardAsync Tests

    [Test]
    public async Task GetInventoryDashboardAsync_WhenNoDataExists_ReturnsOkWithEmptyDashboard()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.That(returnedDashboard, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedDashboard.LowStockItems, Is.Empty);
            Assert.That(returnedDashboard.TotalCustomers, Is.EqualTo(0));
            Assert.That(returnedDashboard.TotalEmployees, Is.EqualTo(0));
            Assert.That(returnedDashboard.TotalCostValue, Is.EqualTo(0));
            Assert.That(returnedDashboard.TotalSalesValue, Is.EqualTo(0));
            Assert.That(returnedDashboard.TotalQuantity, Is.EqualTo(0));
            Assert.That(returnedDashboard.ProfitPotential, Is.EqualTo(0));
        }
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        this.context.BasicCustomers.Add(customer);
        this.context.BasicEmployees.Add(employee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.That(returnedDashboard, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedDashboard.LowStockItems, Is.Not.Empty);
            Assert.That(returnedDashboard.TotalCustomers, Is.EqualTo(1));
            Assert.That(returnedDashboard.TotalEmployees, Is.EqualTo(1));
            Assert.That(returnedDashboard.TotalQuantity, Is.EqualTo(105));
            Assert.That(returnedDashboard.CategoryBreakdown, Has.Count.EqualTo(1));
        }
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.That(returnedDashboard, Is.Not.Null);
        Assert.That(returnedDashboard.LowStockItems, Is.Not.Empty);

        // First item should be the one with the lowest quantity
        Assert.That(returnedDashboard.LowStockItems[0].Name, Is.EqualTo("Low Stock Product"));
    }

    [Test]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryDashboardAsync(ct);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.That(returnedDashboard, Is.Not.Null);

        // Total Cost Value = 10.00 * 10 = 100.00
        // Total Sales Value = 20.00 * 10 = 200.00
        // Profit Potential = 200.00 - 100.00 = 100.00
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedDashboard.TotalCostValue, Is.EqualTo(100.00m));
            Assert.That(returnedDashboard.TotalSalesValue, Is.EqualTo(200.00m));
            Assert.That(returnedDashboard.ProfitPotential, Is.EqualTo(100.00m));
        }
    }

    #endregion

    [Test]
    public void InventoryController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "InventoryController should have Authorize attribute");
    }

    [Test]
    public void InventoryController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "InventoryController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void InventoryController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(InventoryController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "InventoryController should have ApiController attribute");
    }
}

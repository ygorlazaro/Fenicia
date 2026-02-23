using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;

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
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testProductId = Guid.NewGuid();
        this.testCategoryId = Guid.NewGuid();
        this.getInventoryHandler = new GetInventoryHandler(this.context);
        this.getInventoryByProductHandler = new GetInventoryByProductHandler(this.context);
        this.getInventoryByCategoryHandler = new GetInventoryByCategoryHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new InventoryController(
            this.getInventoryHandler,
            this.getInventoryByProductHandler,
            this.getInventoryByCategoryHandler)
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

    private InventoryController controller = null!;
    private BasicContext context = null!;
    private GetInventoryHandler getInventoryHandler = null!;
    private GetInventoryByProductHandler getInventoryByProductHandler = null!;
    private GetInventoryByCategoryHandler getInventoryByCategoryHandler = null!;
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
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page, perPage, cancellationToken);

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

        this.context.ProductCategories.Add(category);
        this.context.Products.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryAsync(page, perPage, cancellationToken);

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

        this.context.ProductCategories.Add(category);
        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(this.testProductId, cancellationToken);

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
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByProductIdAsync(nonExistentId, cancellationToken);

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

        this.context.ProductCategories.Add(category);
        this.context.Products.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(this.testCategoryId, cancellationToken);

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
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetInventoryByCategoryIdAsync(nonExistentId, cancellationToken);

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

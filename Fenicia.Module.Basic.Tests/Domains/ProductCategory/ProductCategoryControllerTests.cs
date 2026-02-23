using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.GetByCategoryId;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Add;
using Fenicia.Module.Basic.Domains.ProductCategory.Delete;
using Fenicia.Module.Basic.Domains.ProductCategory.GetAll;
using Fenicia.Module.Basic.Domains.ProductCategory.GetById;
using Fenicia.Module.Basic.Domains.ProductCategory.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class ProductCategoryControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testCategoryId = Guid.NewGuid();
        this.getAllProductCategoryHandler = new GetAllProductCategoryHandler(this.context);
        this.getProductCategoryByIdHandler = new GetProductCategoryByIdHandler(this.context);
        this.addProductCategoryHandler = new AddProductCategoryHandler(this.context);
        this.updateProductCategoryHandler = new UpdateProductCategoryHandler(this.context);
        this.deleteProductCategoryHandler = new DeleteProductCategoryHandler(this.context);
        this.getProductsByCategoryIdHandler = new GetProductsByCategoryIdHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProductCategoryController(
            this.getAllProductCategoryHandler,
            this.getProductCategoryByIdHandler,
            this.addProductCategoryHandler,
            this.updateProductCategoryHandler,
            this.deleteProductCategoryHandler,
            this.getProductsByCategoryIdHandler)
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

    private ProductCategoryController controller = null!;
    private BasicContext context = null!;
    private GetAllProductCategoryHandler getAllProductCategoryHandler = null!;
    private GetProductCategoryByIdHandler getProductCategoryByIdHandler = null!;
    private AddProductCategoryHandler addProductCategoryHandler = null!;
    private UpdateProductCategoryHandler updateProductCategoryHandler = null!;
    private DeleteProductCategoryHandler deleteProductCategoryHandler = null!;
    private GetProductsByCategoryIdHandler getProductsByCategoryIdHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
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
    public async Task GetAsync_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategories = okResult.Value as List<ProductCategoryResponse>;
        Assert.That(returnedCategories, Is.Not.Null);
        Assert.That(returnedCategories, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenCategoriesExist_ReturnsOkWithCategories()
    {
        // Arrange
        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategories = okResult.Value as List<ProductCategoryResponse>;
        Assert.That(returnedCategories, Is.Not.Null);
        Assert.That(returnedCategories, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsOkWithCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testCategoryId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategory = okResult.Value as ProductCategoryResponse;
        Assert.That(returnedCategory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedCategory.Id, Is.EqualTo(this.testCategoryId));
            Assert.That(returnedCategory.Name, Is.EqualTo(category.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCategory()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), this.faker.Commerce.Categories(1)[0]);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedCategory = createdResult.Value as ProductCategoryResponse;
        Assert.That(returnedCategory, Is.Not.Null);
        Assert.That(returnedCategory.Name, Is.EqualTo(command.Name));
    }

    [Test]
    public async Task PatchAsync_WhenCategoryExists_ReturnsOkWithUpdatedCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(this.testCategoryId, this.faker.Commerce.Categories(1)[0] + " Updated");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testCategoryId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedCategory = okResult.Value as ProductCategoryResponse;
        Assert.That(returnedCategory, Is.Not.Null);
        Assert.That(returnedCategory.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProductCategoryCommand(nonExistentId, this.faker.Commerce.Categories(1)[0]);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_WhenCategoryExists_ReturnsNoContent()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testCategoryId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NoContentResult>());

        var noContentResult = result.Result as NoContentResult;
        Assert.That(noContentResult, Is.Not.Null);
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204));

        // Verify category was deleted
        var deletedCategory = await this.context.ProductCategories.FirstOrDefaultAsync(x => x.Id == this.testCategoryId, cancellationToken);
        Assert.That(deletedCategory, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task GetProductsByCategoryAsync_WhenCategoryHasNoProducts_ReturnsOkWithEmptyList()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetProductsByCategoryAsync(this.testCategoryId, query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<ProductResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Is.Empty);
    }

    [Test]
    public async Task GetProductsByCategoryAsync_WhenCategoryHasProducts_ReturnsOkWithProducts()
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

        var query = new PaginationQuery(1, 10);
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetProductsByCategoryAsync(this.testCategoryId, query, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<ProductResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Has.Count.EqualTo(2));
    }

    [Test]
    public void ProductCategoryController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProductCategoryController should have Authorize attribute");
    }

    [Test]
    public void ProductCategoryController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProductCategoryController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ProductCategoryController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProductCategoryController should have ApiController attribute");
    }
}

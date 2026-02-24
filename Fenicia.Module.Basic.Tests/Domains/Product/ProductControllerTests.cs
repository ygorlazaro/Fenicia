using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.Add;
using Fenicia.Module.Basic.Domains.Product.Delete;
using Fenicia.Module.Basic.Domains.Product.GetAll;
using Fenicia.Module.Basic.Domains.Product.GetById;
using Fenicia.Module.Basic.Domains.Product.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class ProductControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.testProductId = Guid.NewGuid();
        this.getAllProductHandler = new GetAllProductHandler(this.context);
        this.getProductByIdHandler = new GetProductByIdHandler(this.context);
        this.addProductHandler = new AddProductHandler(this.context);
        this.updateProductHandler = new UpdateProductHandler(this.context);
        this.deleteProductHandler = new DeleteProductHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProductController(
            this.getAllProductHandler,
            this.getProductByIdHandler,
            this.addProductHandler,
            this.updateProductHandler,
            this.deleteProductHandler)
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

    private ProductController controller = null!;
    private BasicContext context = null!;
    private GetAllProductHandler getAllProductHandler = null!;
    private GetProductByIdHandler getProductByIdHandler = null!;
    private AddProductHandler addProductHandler = null!;
    private UpdateProductHandler updateProductHandler = null!;
    private DeleteProductHandler deleteProductHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
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
    public async Task GetAsync_WhenNoProductsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<GetAllProductResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenProductsExist_ReturnsOkWithProducts()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
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
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProducts = okResult.Value as List<GetAllProductResponse>;
        Assert.That(returnedProducts, Is.Not.Null);
        Assert.That(returnedProducts, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenProductExists_ReturnsOkWithProduct()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
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
        var result = await this.controller.GetByIdAsync(this.testProductId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProduct = okResult.Value as GetProductByIdResponse;
        Assert.That(returnedProduct, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedProduct.Id, Is.EqualTo(this.testProductId));
            Assert.That(returnedProduct.Name, Is.EqualTo(product.Name));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
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
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithProduct()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddProductCommand(
            category.Id,
            this.faker.Commerce.ProductName(),
            10.00m,
            20.00m,
            100,
            category.Id);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedProduct = createdResult.Value as AddProductResponse;
        Assert.That(returnedProduct, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedProduct.Name, Is.EqualTo(command.Name));
            Assert.That(returnedProduct.CostPrice, Is.EqualTo(command.CostPrice));
            Assert.That(returnedProduct.SalesPrice, Is.EqualTo(command.SellingPrice));
        }
    }

    [Test]
    public async Task PatchAsync_WhenProductExists_ReturnsOkWithUpdatedProduct()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
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

        var command = new UpdateProductCommand(
            product.Id,
            this.faker.Commerce.ProductName() + " Updated",
            15.00m,
            25.00m,
            150,
            category.Id);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProductId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProduct = okResult.Value as UpdateProductResponse;
        Assert.That(returnedProduct, Is.Not.Null);
        Assert.That(returnedProduct.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(
            nonExistentId,
            this.faker.Commerce.ProductName(),
            10.00m,
            20.00m,
            100,
            category.Id);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_WhenProductExists_ReturnsNoContent()
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProductId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify product was deleted
        var deletedProduct = await this.context.Products.FirstOrDefaultAsync (x => x.Id == this.testProductId, cancellationToken);
        Assert.That(deletedProduct, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void ProductController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProductController should have Authorize attribute");
    }

    [Test]
    public void ProductController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProductController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ProductController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProductController should have ApiController attribute");
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.Add;
using Fenicia.Module.Basic.Domains.Product.Delete;
using Fenicia.Module.Basic.Domains.Product.GetAll;
using Fenicia.Module.Basic.Domains.Product.GetById;
using Fenicia.Module.Basic.Domains.Product.GetProductPerformance;
using Fenicia.Module.Basic.Domains.Product.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductControllerTests : IDisposable
{
    public ProductControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testProductId = Guid.NewGuid();
        var getAllProductHandler = new GetAllProductHandler(this.context);
        var getProductByIdHandler = new GetProductByIdHandler(this.context);
        var addProductHandler = new AddProductHandler(this.context);
        var updateProductHandler = new UpdateProductHandler(this.context);
        var deleteProductHandler = new DeleteProductHandler(this.context);
        var getProductPerformanceHandler = new GetProductPerformanceHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProductController(
            getAllProductHandler,
            getProductByIdHandler,
            addProductHandler,
            updateProductHandler,
            deleteProductHandler,
            getProductPerformanceHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    private readonly ProductController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProductId;
    private readonly Faker faker;

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

    [Fact]
    public async Task GetAsync_WhenNoProductsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as Pagination<List<GetAllProductResponse>>;
        Assert.NotNull(returnedProducts);
        Assert.Empty(returnedProducts.Data);
    }

    [Fact]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as Pagination<List<GetAllProductResponse>>;
        Assert.NotNull(returnedProducts);
        Assert.Equal(2, returnedProducts.Data.Count);
    }

    [Fact]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProductId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProduct = okResult.Value as GetProductByIdResponse;
        Assert.NotNull(returnedProduct);
        Assert.Equal(this.testProductId, returnedProduct.Id);
        Assert.Equal(product.Name, returnedProduct.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithProduct()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.BasicProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddProductCommand(
            category.Id,
            this.faker.Commerce.ProductName(),
            10.00m,
            20.00m,
            100,
            category.Id,
            null);

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

        var returnedProduct = createdResult.Value as AddProductResponse;
        Assert.NotNull(returnedProduct);
        Assert.Equal(command.Name, returnedProduct.Name);
        Assert.Equal(command.CostPrice, returnedProduct.CostPrice);
        Assert.Equal(command.SalesPrice, returnedProduct.SalesPrice);
    }

    [Fact]
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

        this.context.BasicProductCategories.Add(category);
        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(
            product.Id,
            this.faker.Commerce.ProductName() + " Updated",
            15.00m,
            25.00m,
            150,
            category.Id,
            null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testProductId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProduct = okResult.Value as UpdateProductResponse;
        Assert.NotNull(returnedProduct);
        Assert.Contains("Updated", returnedProduct.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.context.BasicProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(
            nonExistentId,
            this.faker.Commerce.ProductName(),
            10.00m,
            20.00m,
            100,
            category.Id,
            null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
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

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProductId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify product was deleted
        var deletedProduct = await this.context.BasicProducts.FirstOrDefaultAsync(x => x.Id == this.testProductId && x.Deleted == null, ct);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ProductController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ProductController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ProductController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductController);

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

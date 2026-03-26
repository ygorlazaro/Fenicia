using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductControllerTests : IDisposable
{
    private readonly ProductController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProductId;

    public ProductControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProductId = Guid.NewGuid();
        var getAllProductHandler = new GetAllProductHandler(db);
        var getProductByIdHandler = new GetProductByIdHandler(db);
        var addProductHandler = new AddProductHandler(db);
        var updateProductHandler = new UpdateProductHandler(db);
        var deleteProductHandler = new DeleteProductHandler(db);
        var getProductPerformanceHandler = new GetProductPerformanceHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProductController(getAllProductHandler, getProductByIdHandler, addProductHandler, updateProductHandler, deleteProductHandler, getProductPerformanceHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
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
        var result = await controller.GetAsync(wide, page, perPage, ct);

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
            Name = faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SKU = "SKU001",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SKU = "SKU002",
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 50,
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

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
            Name = faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = testProductId,
            Name = faker.Commerce.ProductName(),
            SKU = "SKU001",
            Barcode = "123456789",
            Description = "Test product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProductId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProduct = okResult.Value as GetProductByIdResponse;
        Assert.NotNull(returnedProduct);
        Assert.Equal(testProductId, returnedProduct.Id);
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
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

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
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddProductCommand(category.Id, faker.Commerce.ProductName(), "SKU001", "123456789", "Test description", 10.00m, 20.00m, 100, 10, 500, "http://test.com/image.jpg", 1.5m, "10x10x10", "un", category.Id, null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

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
            Name = faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = testProductId,
            Name = faker.Commerce.ProductName(),
            SKU = "SKU001",
            Barcode = "123456789",
            Description = "Test product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(product.Id, faker.Commerce.ProductName() + " Updated", "SKU001", "999999999", "Updated description", 15.00m, 25.00m, 150, 20, 600, "http://updated.com", 2.0m, "20x20x20", "kg", category.Id, null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProductId, wide, ct);

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
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(nonExistentId, faker.Commerce.ProductName(), "SKU001", "123456789", "Desc", 10.00m, 20.00m, 100, 10, 500, "http://img.com", 1.5m, "10x10x10", "un", category.Id, null);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var product = new ProductModel
        {
            Id = testProductId,
            Name = faker.Commerce.ProductName(),
            SKU = "SKU001",
            Barcode = "123456789",
            Description = "Test product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            MinStockLevel = 10,
            MaxStockLevel = 500,
            ImageUrl = "http://test.com/image.jpg",
            Weight = 1.5m,
            Dimensions = "10x10x10",
            UnitOfMeasure = "un",
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProductId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify product was deleted
        var deletedProduct = await db.BasicProducts.FirstOrDefaultAsync(x => x.Id == testProductId && x.Deleted == null, ct);
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
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

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
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

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
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryControllerTests : IDisposable
{
    private readonly ProductCategoryController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testCategoryId;

    public ProductCategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testCategoryId = Guid.NewGuid();
        var getAllProductCategoryHandler = new GetAllProductCategoryHandler(db);
        var getProductCategoryByIdHandler = new GetProductCategoryByIdHandler(db);
        var addProductCategoryHandler = new AddProductCategoryHandler(db);
        var updateProductCategoryHandler = new UpdateProductCategoryHandler(db);
        var deleteProductCategoryHandler = new DeleteProductCategoryHandler(db);
        var getProductsByCategoryIdHandler = new GetProductsByCategoryIdHandler(db);
        var sender = new Mock<ISender>();
        sender.Setup(x => x.Send(It.IsAny<GetAllProductCategoryQuery>(), It.IsAny<CancellationToken>())).Returns((GetAllProductCategoryQuery query, CancellationToken ct) => getAllProductCategoryHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<GetProductCategoryByIdQuery>(), It.IsAny<CancellationToken>())).Returns((GetProductCategoryByIdQuery query, CancellationToken ct) => getProductCategoryByIdHandler.Handle(query, ct));
        sender.Setup(x => x.Send(It.IsAny<AddProductCategoryCommand>(), It.IsAny<CancellationToken>())).Returns((AddProductCategoryCommand command, CancellationToken ct) => addProductCategoryHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<UpdateProductCategoryCommand>(), It.IsAny<CancellationToken>())).Returns((UpdateProductCategoryCommand command, CancellationToken ct) => updateProductCategoryHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<DeleteProductCategoryCommand>(), It.IsAny<CancellationToken>())).Returns((DeleteProductCategoryCommand command, CancellationToken ct) => deleteProductCategoryHandler.Handle(command, ct));
        sender.Setup(x => x.Send(It.IsAny<GetProductsByCategoryIdQuery>(), It.IsAny<CancellationToken>())).Returns((GetProductsByCategoryIdQuery query, CancellationToken ct) => getProductsByCategoryIdHandler.Handle(query, ct));
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProductCategoryController(sender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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
    public async Task GetAsync_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as Pagination<List<GetAllProductCategoryResponse>>;
        Assert.NotNull(returnedCategories);
        Assert.Empty(returnedCategories.Data);
        Assert.Equal(0, returnedCategories.Total);
    }

    [Fact]
    public async Task GetAsync_WhenCategoriesExist_ReturnsOkWithCategories()
    {
        // Arrange
        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.AddRange(category1, category2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as Pagination<List<GetAllProductCategoryResponse>>;
        Assert.NotNull(returnedCategories);
        Assert.Equal(2, returnedCategories.Data.Count);
        Assert.Equal(2, returnedCategories.Total);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsOkWithCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testCategoryId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategory = okResult.Value as GetProductCategoryByIdResponse;
        Assert.NotNull(returnedCategory);
        Assert.Equal(testCategoryId, returnedCategory.Id);
        Assert.Equal(category.Name, returnedCategory.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
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
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCategory()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), faker.Commerce.Categories(1)[0]);
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

        var returnedCategory = createdResult.Value as AddProductCategoryResponse;
        Assert.NotNull(returnedCategory);
        Assert.Equal(command.Name, returnedCategory.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryExists_ReturnsOkWithUpdatedCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(testCategoryId, faker.Commerce.Categories(1)[0] + " Updated");
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testCategoryId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategory = okResult.Value as UpdateProductCategoryResponse;
        Assert.NotNull(returnedCategory);
        Assert.Contains("Updated", returnedCategory.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProductCategoryCommand(nonExistentId, faker.Commerce.Categories(1)[0]);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_ReturnsNoContent()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testCategoryId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify category was deleted
        var deletedCategory = await db.BasicProductCategories.FirstOrDefaultAsync(x => x.Id == testCategoryId && x.Deleted == null, ct);
        Assert.Null(deletedCategory);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_ReturnsNoContent()
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
    public async Task GetProductsByCategoryAsync_WhenCategoryHasNoProducts_ReturnsOkWithEmptyList()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetProductsByCategoryAsync(testCategoryId, query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as List<GetProductsByCategoryIdResponse>;
        Assert.NotNull(returnedProducts);
        Assert.Empty(returnedProducts);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WhenCategoryHasProducts_ReturnsOkWithProducts()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 30.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1, 10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await controller.GetProductsByCategoryAsync(testCategoryId, query, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as List<GetProductsByCategoryIdResponse>;
        Assert.NotNull(returnedProducts);
        Assert.Equal(2, returnedProducts.Count);
    }

    [Fact]
    public void ProductCategoryController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ProductCategoryController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ProductCategoryController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }
}

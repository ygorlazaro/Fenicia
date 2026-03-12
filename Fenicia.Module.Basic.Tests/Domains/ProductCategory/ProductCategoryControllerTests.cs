using System.Security.Claims;

using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Responses;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryControllerTests : IDisposable
{
    public ProductCategoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.testCategoryId = Guid.NewGuid();
        var getAllProductCategoryHandler = new GetAllProductCategoryHandler(this.db);
        var getProductCategoryByIdHandler = new GetProductCategoryByIdHandler(this.db);
        var addProductCategoryHandler = new AddProductCategoryHandler(this.db);
        var updateProductCategoryHandler = new UpdateProductCategoryHandler(this.db);
        var deleteProductCategoryHandler = new DeleteProductCategoryHandler(this.db);
        var getProductsByCategoryIdHandler = new GetProductsByCategoryIdHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProductCategoryController(
            getAllProductCategoryHandler,
            getProductCategoryByIdHandler,
            addProductCategoryHandler,
            updateProductCategoryHandler,
            deleteProductCategoryHandler,
            getProductsByCategoryIdHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    private readonly ProductCategoryController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
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
    public async Task GetAsync_WhenNoCategoriesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide,
            page: 1,
            perPage: 10,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as Pagination<List<GetAllProductCategoryResponse>>;
        Assert.NotNull(returnedCategories);
        Assert.Empty(returnedCategories.Data);
        Assert.Equal(0,
            returnedCategories.Total);
    }

    [Fact]
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

        this.db.BasicProductCategories.AddRange(category1,
            category2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide,
            page: 1,
            perPage: 10,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategories = okResult.Value as Pagination<List<GetAllProductCategoryResponse>>;
        Assert.NotNull(returnedCategories);
        Assert.Equal(2,
            returnedCategories.Data.Count);
        Assert.Equal(2,
            returnedCategories.Total);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsOkWithCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testCategoryId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategory = okResult.Value as GetProductCategoryByIdResponse;
        Assert.NotNull(returnedCategory);
        Assert.Equal(this.testCategoryId,
            returnedCategory.Id);
        Assert.Equal(category.Name,
            returnedCategory.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithCategory()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(),
            this.faker.Commerce.Categories(1)[0]);
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

        var returnedCategory = createdResult.Value as AddProductCategoryResponse;
        Assert.NotNull(returnedCategory);
        Assert.Equal(command.Name,
            returnedCategory.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryExists_ReturnsOkWithUpdatedCategory()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(this.testCategoryId,
            this.faker.Commerce.Categories(1)[0] + " Updated");
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command,
            this.testCategoryId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedCategory = okResult.Value as UpdateProductCategoryResponse;
        Assert.NotNull(returnedCategory);
        Assert.Contains("Updated",
            returnedCategory.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenCategoryDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProductCategoryCommand(nonExistentId,
            this.faker.Commerce.Categories(1)[0]);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command,
            nonExistentId,
            wide,
            ct);

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
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testCategoryId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);

        // Verify category was deleted
        var deletedCategory = await this.db.BasicProductCategories.FirstOrDefaultAsync(
            x => x.Id == this.testCategoryId && x.Deleted == null,
            ct);
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
        var result = await this.controller.DeleteAsync(nonExistentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_WhenCategoryHasNoProducts_ReturnsOkWithEmptyList()
    {
        // Arrange
        var category = new ProductCategoryModel
        {
            Id = this.testCategoryId,
            Name = this.faker.Commerce.Categories(1)[0]
        };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new PaginationQuery(1,
            10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductsByCategoryAsync(this.testCategoryId,
            query,
            wide,
            ct);

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

        var query = new PaginationQuery(1,
            10);
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetProductsByCategoryAsync(this.testCategoryId,
            query,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProducts = okResult.Value as List<GetProductsByCategoryIdResponse>;
        Assert.NotNull(returnedProducts);
        Assert.Equal(2,
            returnedProducts.Count);
    }

    [Fact]
    public void ProductCategoryController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ProductCategoryController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

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
    public void ProductCategoryController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProductCategoryController);

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
    }
}

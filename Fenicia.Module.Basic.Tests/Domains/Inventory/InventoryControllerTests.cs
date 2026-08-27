using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryControllerTests : IDisposable
{
    private readonly InventoryController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Mock<ISender> mockSender;
    private readonly Guid testCategoryId;
    private readonly Guid testProductId;
    private readonly WideEventContext wide;

    public InventoryControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProductId = Guid.NewGuid();
        testCategoryId = Guid.NewGuid();
        wide = new WideEventContext();
        var getInventoryHandler = new GetInventoryHandler(db);
        var getInventoryByProductHandler = new GetInventoryByProductHandler(db);
        var getInventoryByCategoryHandler = new GetInventoryByCategoryHandler(db);
        var getInventoryDashboardHandler = new GetInventoryDashboardHandler(db);
        var getInventoryHealthHandler = new GetInventoryHealthHandler(db);
        mockSender = new Mock<ISender>();
        mockHttpContext = new Mock<HttpContext>();

        mockSender.Setup(s => s.Send(It.IsAny<GetInventoryQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInventoryQuery query, CancellationToken ct) => getInventoryHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetInventoryByProductQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInventoryByProductQuery query, CancellationToken ct) => getInventoryByProductHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetInventoryByCategoryQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInventoryByCategoryQuery query, CancellationToken ct) => getInventoryByCategoryHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetInventoryDashboardQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInventoryDashboardQuery query, CancellationToken ct) => getInventoryDashboardHandler.Handle(query, ct));

        mockSender.Setup(s => s.Send(It.IsAny<GetInventoryHealthQuery>(), It.IsAny<CancellationToken>()))
            .Returns((GetInventoryHealthQuery query, CancellationToken ct) => getInventoryHealthHandler.Handle(query, ct));

        controller = new InventoryController(mockSender.Object) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
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
    public async Task GetInventoryAsync_WhenNoProductsExist_ReturnsOkWithEmptyInventory()
    {

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        var result = await controller.GetInventoryAsync(wide, page, perPage, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Empty(returnedInventory.Items);
        Assert.Equal(0, returnedInventory.TotalCostPrice);
        Assert.Equal(0, returnedInventory.TotalSalesPrice);
        Assert.Equal(0, returnedInventory.TotalQuantity);
    }

    [Fact]
    public async Task GetInventoryAsync_WhenProductsExist_ReturnsOkWithInventory()
    {

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

        var page = 1;
        var perPage = 10;
        var ct = CancellationToken.None;

        var result = await controller.GetInventoryAsync(wide, page, perPage, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Equal(2, returnedInventory.Items.Count);
        Assert.Equal(25.00m, returnedInventory.TotalCostPrice);
        Assert.Equal(50.00m, returnedInventory.TotalSalesPrice);
        Assert.Equal(150, returnedInventory.TotalQuantity);
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_WhenProductExists_ReturnsOkWithInventory()
    {

        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = testProductId,
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryByProductIdAsync(testProductId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Single(returnedInventory.Items);
        Assert.Equal(testProductId, returnedInventory.Items[0].Id);
        Assert.Equal(product.Name, returnedInventory.Items[0].Name);
    }

    [Fact]
    public async Task GetInventoryByProductIdAsync_WhenProductDoesNotExist_ReturnsOkWithEmptyInventory()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var result = await controller.GetInventoryByProductIdAsync(nonExistentId, wide, ct);

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

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryByCategoryIdAsync(testCategoryId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Equal(2, returnedInventory.Items.Count);
        Assert.Equal(category.Id, returnedInventory.Items[0].CategoryId);
        Assert.Equal(category.Id, returnedInventory.Items[1].CategoryId);
    }

    [Fact]
    public async Task GetInventoryByCategoryIdAsync_WhenCategoryDoesNotExist_ReturnsOkWithEmptyInventory()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var result = await controller.GetInventoryByCategoryIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedInventory = okResult.Value as InventoryResponse;
        Assert.NotNull(returnedInventory);
        Assert.Empty(returnedInventory.Items);
    }

    [Fact]
    public void InventoryController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(InventoryController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void InventoryController_HasRouteAttribute()
    {

        var controllerType = typeof(InventoryController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void InventoryController_HasApiControllerAttribute()
    {

        var controllerType = typeof(InventoryController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    #region GetInventoryDashboardAsync Tests

    [Fact]
    public async Task GetInventoryDashboardAsync_WhenNoDataExists_ReturnsOkWithEmptyDashboard()
    {

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryDashboardAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.Empty(returnedDashboard.LowStockItems);
        Assert.Equal(0, returnedDashboard.TotalCustomers);
        Assert.Equal(0, returnedDashboard.TotalEmployees);
        Assert.Equal(0, returnedDashboard.TotalCostValue);
        Assert.Equal(0, returnedDashboard.TotalSalesValue);
        Assert.Equal(0, returnedDashboard.TotalQuantity);
        Assert.Equal(0, returnedDashboard.ProfitPotential);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_WhenProductsExist_ReturnsOkWithDashboardData()
    {

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
            Quantity = 5,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
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
                Name = faker.Name.FullName()
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
                Name = faker.Name.FullName()
            }
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.AddRange(product1, product2);
        db.BasicCustomers.Add(customer);
        db.BasicEmployees.Add(employee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryDashboardAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.NotEmpty(returnedDashboard.LowStockItems);
        Assert.Equal(1, returnedDashboard.TotalCustomers);
        Assert.Equal(1, returnedDashboard.TotalEmployees);
        Assert.Equal(105, returnedDashboard.TotalQuantity);
        Assert.Single(returnedDashboard.CategoryBreakdown);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_ReturnsLowStockItemsOrderedByQuantity()
    {

        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
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

        db.BasicProductCategories.Add(category);
        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryDashboardAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);
        Assert.NotEmpty(returnedDashboard.LowStockItems);

        Assert.Equal("Low Stock Product", returnedDashboard.LowStockItems[0].Name);
    }

    [Fact]
    public async Task GetInventoryDashboardAsync_CalculatesProfitPotentialCorrectly()
    {

        var category = new ProductCategoryModel
        {
            Id = testCategoryId,
            Name = faker.Commerce.Categories(1)[0]
        };

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 10,
            CategoryId = category.Id
        };

        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var result = await controller.GetInventoryDashboardAsync(wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedDashboard = okResult.Value as InventoryDashboardResponse;
        Assert.NotNull(returnedDashboard);

        Assert.Equal(100.00m, returnedDashboard.TotalCostValue);
        Assert.Equal(200.00m, returnedDashboard.TotalSalesValue);
        Assert.Equal(100.00m, returnedDashboard.ProfitPotential);
    }

    #endregion
}

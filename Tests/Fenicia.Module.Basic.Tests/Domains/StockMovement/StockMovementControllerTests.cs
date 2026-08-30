using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Tests.Domains.StockMovement;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementControllerTests : IDisposable
{
    private readonly StockMovementController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;

    public StockMovementControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new TestCompanyContext());
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        var service = new StockMovementService(new StockMovementRepository(_db), productService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new StockMovementController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = category.Id };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 5.0, DateTime.UtcNow, 100, StockMovementType.In, product.Id, null, null, null, null, "Test");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenMovementExists_ReturnsOk()
    {
        // Arrange
        var movement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 5, Date = DateTime.UtcNow, Price = 100, Type = StockMovementType.In };
        _db.BasicStockMovements.Add(movement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(movement.Id, Quantity: 10.0, Date: DateTime.UtcNow, Price: 100, Type: StockMovementType.Out, ProductId: movement.ProductId, CustomerId: null, SupplierId: null, EmployeeId: null, OrderId: null, Reason: "Updated");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(movement.Id, command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenMovementDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateStockMovementCommand(Guid.NewGuid(), Quantity: 10.0, Date: DateTime.UtcNow, Price: 100, Type: StockMovementType.Out, ProductId: Guid.NewGuid(), CustomerId: null, SupplierId: null, EmployeeId: null, OrderId: null, Reason: "Updated");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(Guid.NewGuid(), command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAsync_WhenStockMovementsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, null, null, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetDashboardAsync(wide, 30, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Tests.Domains.Product;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductControllerTests : IDisposable
{
    private readonly ProductController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TestCompanyContext _companyContext;

    public ProductControllerTests()
    {
        _companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, _companyContext);
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var service = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProductController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
        var command = new AddProductCommand(Id: Guid.NewGuid(), Name: _faker.Commerce.ProductName(), SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProductExists_ReturnsOk()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = _companyContext.CompanyId };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(product.Id, Name: "Updated Name", SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, product.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateProductCommand(Id: Guid.NewGuid(), Name: "Updated Name", SalesPrice: _faker.Random.Decimal());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = _companyContext.CompanyId };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(product.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenProductsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsOk()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), CompanyId = _companyContext.CompanyId };
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), CategoryId = category.Id, CompanyId = _companyContext.CompanyId };
        _db.BasicProductCategories.Add(category);
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(product.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
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

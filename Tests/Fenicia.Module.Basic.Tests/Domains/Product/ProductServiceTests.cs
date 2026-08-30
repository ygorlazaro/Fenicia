using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProductRepository(_db);
        _service = new ProductService(repository, new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(new OrderDetailRepository(_db)), new StockMovementService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ReturnsPaginationWithProducts()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal(), CategoryId = category.Id };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProductQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal(), CategoryId = category.Id };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProductByIdQuery(product.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesProduct()
    {
        // Arrange
        var command = new AddProductCommand(Id: Guid.NewGuid(), Name: _faker.Commerce.ProductName(), SalesPrice: _faker.Random.Decimal());

        // Act
        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(product.Id, Name: "Updated Name", SalesPrice: _faker.Random.Decimal());

        // Act
        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(Id: Guid.NewGuid(), Name: "Updated Name", SalesPrice: _faker.Random.Decimal());

        // Act
        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SoftDeletesProduct()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProductCommand(product.Id), _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        var deletedProduct = await _db.BasicProducts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        deletedProduct.Should().NotBeNull();
        deletedProduct!.Deleted.Should().NotBeNull();
    }
}

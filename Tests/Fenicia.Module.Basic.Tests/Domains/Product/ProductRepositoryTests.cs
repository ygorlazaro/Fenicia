using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class ProductRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new ProductRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenProductIsValid_InsertsProduct()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };

        // Act
        var result = await _repository.InsertAsync(product, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        product.Name = "Updated Name";
        var result = await _repository.UpdateAsync(product.Id, product, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };

        // Act
        var result = await _repository.UpdateAsync(product.Id, product, CancellationToken.None);

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
        var result = await _repository.DeleteAsync(product.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedProduct = await _db.BasicProducts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        deletedProduct.Should().NotBeNull();
        deletedProduct!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingProducts()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(p => p.Id == product.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenProductExists_ReturnsTrue()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(p => p.Id == product.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(p => p.Id == product.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}

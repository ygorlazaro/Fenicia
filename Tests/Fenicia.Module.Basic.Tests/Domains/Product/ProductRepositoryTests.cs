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
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenProductIsValid_InsertsProduct()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };

        var result = await _repository.InsertAsync(product, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesProduct()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        product.Name = "Updated Name";
        var result = await _repository.UpdateAsync(product.Id, product, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };

        var result = await _repository.UpdateAsync(product.Id, product, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SoftDeletesProduct()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(product.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedProduct = await _db.BasicProducts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(deletedProduct);
        Assert.NotNull(deletedProduct.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingProducts()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(p => p.Id == product.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenProductExists_ReturnsTrue()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(p => p.Id == product.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = _faker.Random.Decimal() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(p => p.Id == product.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}

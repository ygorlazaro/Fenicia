using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProductCategoryRepository _repository;

    public ProductCategoryRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new ProductCategoryRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProductCategories()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductCategoryExists_ReturnsProductCategory()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(productCategory.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(productCategory.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductCategoryDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenProductCategoryIsValid_InsertsProductCategory()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };

        var result = await _repository.InsertAsync(productCategory, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductCategoryExists_UpdatesProductCategory()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(productCategory.Id, productCategory, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(productCategory.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductCategoryDoesNotExist_ReturnsNull()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };

        var result = await _repository.UpdateAsync(productCategory.Id, productCategory, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductCategoryExists_SoftDeletesProductCategory()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(productCategory.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedProductCategory = await _db.BasicProductCategories.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == productCategory.Id);
        Assert.NotNull(deletedProductCategory);
        Assert.NotNull(deletedProductCategory.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingProductCategories()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(p => p.Id == productCategory.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenProductCategoryExists_ReturnsTrue()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(p => p.Id == productCategory.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var productCategory = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(productCategory);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(p => p.Id == productCategory.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class ProductCategoryServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProductCategoryService _service;

    public ProductCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProductCategoryRepository(_db);
        _service = new ProductCategoryService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsPaginationWithCategories()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllProductCategoryQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetProductCategoryByIdQuery(category.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetProductCategoryByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesCategory()
    {
        var command = new AddProductCategoryCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First());

        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryExists_UpdatesCategory()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(category.Id, "Updated Name");

        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Updated Name");

        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_SoftDeletesCategory()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First() };
        _db.BasicProductCategories.Add(category);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeleteProductCategoryCommand(category.Id), _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        var deletedCategory = await _db.BasicProductCategories.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == category.Id);
        Assert.NotNull(deletedCategory);
        Assert.NotNull(deletedCategory.Deleted);
    }
}

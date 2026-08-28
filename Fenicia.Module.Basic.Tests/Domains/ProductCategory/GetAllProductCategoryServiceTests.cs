using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class GetAllProductCategoryServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductCategoryService service;

    public GetAllProductCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProductCategoryService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoCategories_ReturnsEmptyPagination()
    {
        var result = await service.GetAllAsync(new GetAllProductCategoryQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsPaginationWithCategories()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAllAsync(new GetAllProductCategoryQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
    }
}

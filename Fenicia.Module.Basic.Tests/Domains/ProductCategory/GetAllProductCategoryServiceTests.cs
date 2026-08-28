using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Total);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        await db.SaveChangesAsync(CancellationToken.None);
        db.BasicProductCategories.Add(category);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductCategoryService service;
    public async Task GetAllAsync_WhenCategoriesExist_ReturnsPaginationWithCategories()
    public async Task GetAllAsync_WhenNoCategories_ReturnsEmptyPagination()
public class GetAllProductCategoryServiceTests : IDisposable
    public GetAllProductCategoryServiceTests()
    public void Dispose()
        service = new ProductCategoryService(productCategoryRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productCategoryRepository = new ProductCategoryRepository(db);
        var result = await service.GetAllAsync(new GetAllProductCategoryQuery(1, 10), CancellationToken.None);

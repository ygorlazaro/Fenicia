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
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(category.Name, result.Name);
        Assert.NotNull(result);
        Assert.Null(result);
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
    public async Task GetByIdAsync_WhenCategoryDoesNotExist_ReturnsNull()
    public async Task GetByIdAsync_WhenCategoryExists_ReturnsCategory()
public class GetProductCategoryByIdServiceTests : IDisposable
    public GetProductCategoryByIdServiceTests()
    public void Dispose()
        service = new ProductCategoryService(productCategoryRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productCategoryRepository = new ProductCategoryRepository(db);
        var result = await service.GetByIdAsync(new GetProductCategoryByIdQuery(category.Id), CancellationToken.None);
        var result = await service.GetByIdAsync(new GetProductCategoryByIdQuery(Guid.NewGuid()), CancellationToken.None);

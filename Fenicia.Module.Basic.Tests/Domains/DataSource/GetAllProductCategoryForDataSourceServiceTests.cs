using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
        db.BasicProductCategories.Add(category);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.DataSource;
    private readonly DataSourceService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    public async Task GetProductCategoriesAsync_WhenCategoriesExist_ReturnsCategories()
    public async Task GetProductCategoriesAsync_WhenNoCategories_ReturnsEmptyList()
public class GetAllProductCategoryForDataSourceServiceTests : IDisposable
    public GetAllProductCategoryForDataSourceServiceTests()
    public void Dispose()
        service = new DataSourceService(db);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await service.GetProductCategoriesAsync(CancellationToken.None);

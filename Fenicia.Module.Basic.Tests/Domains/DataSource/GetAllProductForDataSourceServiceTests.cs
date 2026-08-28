using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId = category.Id,
        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            IsActive = true
            Name = faker.Commerce.ProductName(),
namespace Fenicia.Module.Basic.Tests.Domains.DataSource;
    private readonly DataSourceService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    public async Task GetProductsAsync_WhenNoProducts_ReturnsEmptyList()
    public async Task GetProductsAsync_WhenProductsExist_ReturnsProducts()
public class GetAllProductForDataSourceServiceTests : IDisposable
    public GetAllProductForDataSourceServiceTests()
    public void Dispose()
            Quantity = faker.Random.Int(1, 100),
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new DataSourceService(db);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var product = new ProductModel
        var result = await service.GetProductsAsync(CancellationToken.None);

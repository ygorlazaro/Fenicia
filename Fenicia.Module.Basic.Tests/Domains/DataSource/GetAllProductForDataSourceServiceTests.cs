using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class GetAllProductForDataSourceServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DataSourceService service;

    public GetAllProductForDataSourceServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new DataSourceService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetProductsAsync_WhenNoProducts_ReturnsEmptyList()
    {
        var result = await service.GetProductsAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductsAsync_WhenProductsExist_ReturnsProducts()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = faker.Random.Int(1, 100),
            CategoryId = category.Id,
            IsActive = true
        };
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetProductsAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}

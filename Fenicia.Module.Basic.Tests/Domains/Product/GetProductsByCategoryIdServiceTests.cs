using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductsByCategoryIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductService service;

    public GetProductsByCategoryIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProductService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WhenCategoryHasProducts_ReturnsProducts()
    {
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = faker.Random.Int(1, 100),
            CategoryId = categoryId,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(categoryId), 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WhenCategoryHasNoProducts_ReturnsEmptyList()
    {
        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(Guid.NewGuid()), 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

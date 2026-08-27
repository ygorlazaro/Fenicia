using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryByProductServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly InventoryService service;

    public GetInventoryByProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new InventoryService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByProductAsync_WhenProductExists_ReturnsInventory()
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

        var result = await service.GetByProductAsync(new GetInventoryByProductQuery(product.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Single(result.Items);
    }
}

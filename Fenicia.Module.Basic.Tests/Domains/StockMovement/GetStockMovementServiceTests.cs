using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Product;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class GetStockMovementServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly StockMovementService service;

    public GetStockMovementServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var stockMovementRepository = new StockMovementRepository(db);
        var productRepository = new ProductRepository(db);
        service = new StockMovementService(stockMovementRepository, productRepository);
        faker = new Faker();
        var companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenNoMovements_ReturnsEmptyList()
    {
        var result = await service.GetAsync(new GetStockMovementQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAsync_WhenMovementsExist_ReturnsMovements()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var movement = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = 10,
            Date = DateTime.UtcNow,
            Price = 10,
            Type = StockMovementType.In,
            Reason = "Test"
        };
        db.BasicStockMovements.Add(movement);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAsync(new GetStockMovementQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}

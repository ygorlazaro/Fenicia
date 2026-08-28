using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement;
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
            Date = DateTime.UtcNow,
        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        db.BasicStockMovements.Add(movement);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            IsActive = true
            Name = faker.Commerce.ProductName(),
namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;
            Price = 10,
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StockMovementService service;
            ProductId = product.Id,
    public async Task GetAsync_WhenMovementsExist_ReturnsMovements()
    public async Task GetAsync_WhenNoMovements_ReturnsEmptyList()
public class GetStockMovementServiceTests : IDisposable
    public GetStockMovementServiceTests()
    public void Dispose()
            Quantity = 10,
            Quantity = 100,
            Reason = "Test"
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new StockMovementService(stockMovementRepository, productRepository);
            Type = StockMovementType.In,
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var movement = new StockMovementModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.GetAsync(new GetStockMovementQuery(DateTime.UtcNow.AddDays(-30), DateTime.UtcNow, 1, 10), CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);

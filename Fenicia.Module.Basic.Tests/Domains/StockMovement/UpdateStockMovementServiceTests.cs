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
        Assert.Equal(20, result.Quantity);
        Assert.Equal(movement.Id, result.Id);
        Assert.NotNull(result);
        Assert.Null(result);
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
    public async Task UpdateAsync_WhenMovementDoesNotExist_ReturnsNull()
    public async Task UpdateAsync_WhenMovementExists_ReturnsUpdateResponse()
public class UpdateStockMovementServiceTests : IDisposable
    public UpdateStockMovementServiceTests()
    public void Dispose()
            Quantity = 10,
            Quantity = 100,
            Reason = "Test"
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new StockMovementService(stockMovementRepository, productRepository);
            Type = StockMovementType.In,
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new UpdateStockMovementCommand(Guid.NewGuid(), 10, DateTime.UtcNow, 10, StockMovementType.In, Guid.NewGuid(), null, null, null, null, "Test");
        var command = new UpdateStockMovementCommand(movement.Id, 20, DateTime.UtcNow, 15, StockMovementType.Out, product.Id, null, null, null, null, "Updated");
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var movement = new StockMovementModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);

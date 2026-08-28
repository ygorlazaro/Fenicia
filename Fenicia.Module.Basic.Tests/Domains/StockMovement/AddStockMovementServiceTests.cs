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
            10,
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.ProductId, result.ProductId);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId = category.Id,
            DateTime.UtcNow,
        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
            faker.Random.Decimal(10, 100),
        GC.SuppressFinalize(this);
            Guid.NewGuid(),
            Id = Guid.NewGuid(),
            "Initial stock");
            IsActive = true
            Name = faker.Commerce.ProductName(),
namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;
            null, null, null, null,
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StockMovementService service;
            product.Id,
    public AddStockMovementServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddStockMovementResponse()
public class AddStockMovementServiceTests : IDisposable
    public void Dispose()
            Quantity = 100,
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new StockMovementService(stockMovementRepository, productRepository);
            StockMovementType.In,
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new AddStockMovementCommand(
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.AddAsync(command, companyId, CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);

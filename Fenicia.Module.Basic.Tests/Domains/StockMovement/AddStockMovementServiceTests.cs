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

public class AddStockMovementServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly StockMovementService service;

    public AddStockMovementServiceTests()
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
    public async Task AddAsync_WithValidCommand_ReturnsAddStockMovementResponse()
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

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.UtcNow,
            faker.Random.Decimal(10, 100),
            StockMovementType.In,
            product.Id,
            null, null, null, null,
            "Initial stock");

        var result = await service.AddAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.ProductId, result.ProductId);
    }
}

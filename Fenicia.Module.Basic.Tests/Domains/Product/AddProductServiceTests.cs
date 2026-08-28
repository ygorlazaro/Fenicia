using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId: category.Id,
        db.BasicProductCategories.Add(category);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
            faker.Commerce.ProductName(),
        faker = new Faker();
        GC.SuppressFinalize(this);
            Guid.NewGuid(),
namespace Fenicia.Module.Basic.Tests.Domains.Product;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductService service;
    public AddProductServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddProductResponse()
public class AddProductServiceTests : IDisposable
    public void Dispose()
            Quantity: faker.Random.Int(1, 100));
            SalesPrice: faker.Random.Decimal(10, 100),
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new AddProductCommand(
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var productRepository = new ProductRepository(db);
        var result = await service.AddAsync(command, companyId, CancellationToken.None);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);

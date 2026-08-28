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
        };
    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId = categoryId,
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
namespace Fenicia.Module.Basic.Tests.Domains.Product;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductService service;
    public async Task GetByCategoryIdAsync_WhenCategoryHasNoProducts_ReturnsEmptyList()
    public async Task GetByCategoryIdAsync_WhenCategoryHasProducts_ReturnsProducts()
public class GetProductsByCategoryIdServiceTests : IDisposable
    public GetProductsByCategoryIdServiceTests()
    public void Dispose()
            Quantity = faker.Random.Int(1, 100),
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(categoryId), 1, 10, CancellationToken.None);
        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(Guid.NewGuid()), 1, 10, CancellationToken.None);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);

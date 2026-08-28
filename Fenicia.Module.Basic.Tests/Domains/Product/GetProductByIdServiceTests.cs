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
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.NotNull(result);
        Assert.Null(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId = category.Id,
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
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
public class GetProductByIdServiceTests : IDisposable
    public GetProductByIdServiceTests()
    public void Dispose()
            Quantity = faker.Random.Int(1, 100),
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.GetByIdAsync(new GetProductByIdQuery(Guid.NewGuid()), CancellationToken.None);
        var result = await service.GetByIdAsync(new GetProductByIdQuery(product.Id), CancellationToken.None);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);

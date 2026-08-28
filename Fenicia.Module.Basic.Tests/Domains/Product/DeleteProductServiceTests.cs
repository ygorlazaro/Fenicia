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
        Assert.Equal(0, count);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
        await db.SaveChangesAsync(CancellationToken.None);
        await service.DeleteAsync(new DeleteProductCommand(Guid.NewGuid()), companyId, CancellationToken.None);
        await service.DeleteAsync(new DeleteProductCommand(product.Id), companyId, CancellationToken.None);
            CategoryId = Guid.NewGuid(),
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
    public async Task DeleteAsync_WhenProductDoesNotExist_DoesNothing()
    public async Task DeleteAsync_WhenProductExists_SetsDeletedDate()
public class DeleteProductServiceTests : IDisposable
    public DeleteProductServiceTests()
    public void Dispose()
            Quantity = faker.Random.Int(1, 100),
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var count = await db.BasicProducts.CountAsync();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);
        var updated = await db.BasicProducts.FindAsync(product.Id);

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
        Assert.Equal(newName, result.Name);
        Assert.Equal(product.Id, result.Id);
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
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    public async Task UpdateAsync_WhenProductExists_ReturnsUpdateResponse()
public class UpdateProductServiceTests : IDisposable
    public UpdateProductServiceTests()
    public void Dispose()
            Quantity = faker.Random.Int(1, 100),
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new UpdateProductCommand(Guid.NewGuid(), faker.Commerce.ProductName(), CategoryId: Guid.NewGuid(), SalesPrice: 10, Quantity: 1);
        var command = new UpdateProductCommand(product.Id, newName, CategoryId: category.Id, SalesPrice: product.SalesPrice, Quantity: product.Quantity);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var newName = faker.Commerce.ProductName();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);

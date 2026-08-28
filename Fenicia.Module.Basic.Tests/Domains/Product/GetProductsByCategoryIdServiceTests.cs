using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.ProductCategory;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductsByCategoryIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly ProductService service;

    public GetProductsByCategoryIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var productRepository = new ProductRepository(db);
        var productCategoryRepository = new ProductCategoryRepository(db);
        var supplierRepository = new SupplierRepository(db);
        var orderDetailRepository = new Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository(db);
        var stockMovementRepository = new Fenicia.Module.Basic.Domains.StockMovement.StockMovementRepository(db);
        service = new ProductService(productRepository, productCategoryRepository, supplierRepository, orderDetailRepository, stockMovementRepository);
        faker = new Faker();
        var companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WhenCategoryHasProducts_ReturnsProducts()
    {
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = faker.Random.Int(1, 100),
            CategoryId = categoryId,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(categoryId), 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_WhenCategoryHasNoProducts_ReturnsEmptyList()
    {
        var result = await service.GetByCategoryIdAsync(new GetProductsByCategoryIdQuery(Guid.NewGuid()), 1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

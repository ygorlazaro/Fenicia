using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.ProductCategory;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class UpdateProductServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly ProductService service;

    public UpdateProductServiceTests()
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
    public async Task UpdateAsync_WhenProductExists_ReturnsUpdateResponse()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = faker.Random.Int(1, 100),
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var newName = faker.Commerce.ProductName();
        var command = new UpdateProductCommand(product.Id, newName, CategoryId: category.Id, SalesPrice: product.SalesPrice, Quantity: product.Quantity);

        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(newName, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), faker.Commerce.ProductName(), CategoryId: Guid.NewGuid(), SalesPrice: 10, Quantity: 1);

        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

        Assert.Null(result);
    }
}

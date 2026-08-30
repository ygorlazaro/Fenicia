using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;
using OrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class DebugProductUpdateTest : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    public DebugProductUpdateTest()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DebugProductUpdate()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var allProducts = await _db.BasicProducts.ToListAsync();
        var savedProduct = await _db.BasicProducts.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(savedProduct);
        Assert.Equal(_db.CurrentCompanyId, savedProduct.CompanyId);

        var productService = new ProductService(
            new ProductRepository(_db),
            new ProductCategoryService(new ProductCategoryRepository(_db)),
            new SupplierRepository(_db),
            new OrderDetailRepository(_db),
            new StockMovementRepository(_db));

        var getResult = await productService.GetByIdAsync(new Fenicia.Module.Basic.Domains.Product.DTOs.GetProductByIdQuery(product.Id), CancellationToken.None);
        Assert.NotNull(getResult);

        var updateCommand = new Fenicia.Module.Basic.Domains.Product.DTOs.UpdateProductCommand(
            getResult.Id,
            getResult.Name,
            Description: getResult.Description,
            CostPrice: getResult.CostPrice,
            SalesPrice: getResult.SalesPrice,
            Quantity: 15.0,
            CategoryId: getResult.CategoryId,
            SupplierId: getResult.SupplierId);

        var updateResult = await productService.UpdateAsync(updateCommand, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);
        Assert.NotNull(updateResult);

        var updatedProduct = await _db.BasicProducts.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(15, updatedProduct.Quantity);
    }
}

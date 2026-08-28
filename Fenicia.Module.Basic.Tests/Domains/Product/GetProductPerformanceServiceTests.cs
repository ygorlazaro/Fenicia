using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Supplier;
using Fenicia.Module.Basic.Domains.ProductCategory;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductPerformanceServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly ProductService service;

    public GetProductPerformanceServiceTests()
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
    public async Task GetPerformanceAsync_ReturnsPerformanceResponse()
    {
        var result = await service.GetPerformanceAsync(new GetProductPerformanceQuery(90, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.BestSellingProducts);
        Assert.NotNull(result.WorstSellingProducts);
        Assert.NotNull(result.ProfitMargins);
        Assert.NotNull(result.NeverSoldProducts);
    }
}

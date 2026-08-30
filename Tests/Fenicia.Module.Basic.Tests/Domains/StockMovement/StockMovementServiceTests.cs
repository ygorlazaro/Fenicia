using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;
using OrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly StockMovementService _service;

    public StockMovementServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var stockMovementRepository = new StockMovementRepository(_db);
        var orderDetailService = new OrderDetailService(new OrderDetailRepository(_db));
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(
            new ProductRepository(_db),
            new ProductCategoryService(new ProductCategoryRepository(_db)),
            orderDetailService,
            dummyStockMovementService);
        var stockMovementService = new StockMovementService(stockMovementRepository, productService);
        _service = new StockMovementService(stockMovementRepository, productService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenMovementsExist_ReturnsMovements()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        var movement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 5, Date = DateTime.UtcNow, Price = 100, Type = StockMovementType.In, CompanyId = Guid.NewGuid() };
        _db.BasicStockMovements.Add(movement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAsync(new GetStockMovementQuery(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task AddAsync_WhenValid_InsertsMovementAndUpdatesProductQuantity()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 5.0, DateTime.UtcNow, 100, StockMovementType.In, product.Id, null, null, null, null, "Test");

        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.ProductId);

        var updatedProduct = await _db.BasicProducts.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equal(15, updatedProduct.Quantity);
    }

    [Fact]
    public async Task UpdateAsync_WhenMovementExists_UpdatesMovement()
    {
        var movement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = Guid.NewGuid(), Quantity = 5, Date = DateTime.UtcNow, Type = StockMovementType.In, CompanyId = Guid.NewGuid() };
        _db.BasicStockMovements.Add(movement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(movement.Id, Quantity: 10.0, Date: DateTime.UtcNow, Price: 100, Type: StockMovementType.Out, ProductId: movement.ProductId, CustomerId: null, SupplierId: null, EmployeeId: null, OrderId: null, Reason: "Updated");

        var result = await _service.UpdateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Quantity);
    }

    [Fact]
    public async Task GetDashboardAsync_WhenDataExists_ReturnsDashboard()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        var movement = new StockMovementModel { Id = Guid.NewGuid(), ProductId = product.Id, Quantity = 5, Date = DateTime.UtcNow.AddDays(-1), Type = StockMovementType.In, CompanyId = Guid.NewGuid() };
        _db.BasicStockMovements.Add(movement);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetDashboardAsync(new GetStockMovementDashboardQuery(30, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.History);
        Assert.NotNull(result.MonthlyInOut);
        Assert.NotNull(result.TopMovedProducts);
        Assert.NotNull(result.TurnoverRates);
    }
}

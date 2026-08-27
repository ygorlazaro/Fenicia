using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;
using Fenicia.Module.Basic.Domains.StockMovement.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class GetStockMovementHandlerTests : IDisposable
{
    private readonly TestCompanyContext companyContext;
    private readonly DefaultContext db;
    private readonly GetStockMovementHandler handler;

    public GetStockMovementHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetStockMovementHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithNoMovementsInDateRange_ReturnsEmptyList()
    {

        var startDate = DateTime.Now.AddDays(-10);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithMovementsInDateRange_ReturnsFilteredList()
    {

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        db.BasicProducts.Add(product);

        var movement1 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id,
            Reason = "Test reason 1"
        };

        var movement2 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 20,
            Date = DateTime.Now.AddDays(-2),
            Price = 25.00m,
            Type = StockMovementType.Out,
            ProductId = product.Id,
            Reason = "Test reason 2"
        };

        var movement3 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 30,
            Date = DateTime.Now.AddDays(5),
            Price = 35.00m,
            Type = StockMovementType.In,
            ProductId = product.Id,
            Reason = null
        };

        db.BasicStockMovements.AddRange(movement1, movement2, movement3);
        await db.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-10);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.All(m => m.Date >= startDate && m.Date <= endDate));
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        db.BasicProducts.Add(product);

        for (var i = 0; i < 25; i++)
        {
            var movement = new StockMovementModel
            {
                Id = Guid.NewGuid(),
                Quantity = 10,
                Date = DateTime.Now.AddDays(-i),
                Price = 15.00m,
                Type = StockMovementType.In,
                ProductId = product.Id
            };
            db.BasicStockMovements.Add(movement);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate, 2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        db.BasicProducts.Add(product);

        for (var i = 0; i < 5; i++)
        {
            var movement = new StockMovementModel
            {
                Id = Guid.NewGuid(),
                Quantity = 10,
                Date = DateTime.Now.AddDays(-i),
                Price = 15.00m,
                Type = StockMovementType.In,
                ProductId = product.Id
            };
            db.BasicStockMovements.Add(movement);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate, 10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

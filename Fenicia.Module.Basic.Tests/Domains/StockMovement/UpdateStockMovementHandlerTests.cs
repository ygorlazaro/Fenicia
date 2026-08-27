using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class UpdateStockMovementHandlerTests : IDisposable
{
    private readonly TestCompanyContext companyContext;
    private readonly DefaultContext db;
    private readonly UpdateStockMovementHandler handler;

    public UpdateStockMovementHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateStockMovementHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenStockMovementExists_UpdatesStockMovementAndReturnsResponse()
    {

        var movementId = Guid.NewGuid();
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

        var movement = new StockMovementModel
        {
            Id = movementId,
            Quantity = 10,
            Date = DateTime.Now,
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };
        db.BasicStockMovements.Add(movement);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(movementId, 20, DateTime.Now.AddDays(1), 25.00m, StockMovementType.Out, product.Id, null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(20, result.Quantity);
        Assert.Equal(StockMovementType.Out, result.Type);
        Assert.Equal(25.00m, result.Price);
    }

    [Fact]
    public async Task Handle_WhenStockMovementDoesNotExist_ReturnsNull()
    {

        var command = new UpdateStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, Guid.NewGuid(), null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var command = new UpdateStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, Guid.NewGuid(), null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesStockMovementWasUpdatedInDatabase()
    {

        var movementId = Guid.NewGuid();
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

        var movement = new StockMovementModel
        {
            Id = movementId,
            Quantity = 10,
            Date = DateTime.Now,
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };
        db.BasicStockMovements.Add(movement);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(movementId, 20, DateTime.Now.AddDays(1), 25.00m, StockMovementType.Out, product.Id, null, null, null, null, null);

        await handler.Handle(command, CancellationToken.None);

        var updatedMovement = await db.BasicStockMovements.FindAsync([movementId], CancellationToken.None);
        Assert.NotNull(updatedMovement);
        Assert.Equal(20, updatedMovement.Quantity);
        Assert.Equal(StockMovementType.Out, updatedMovement.Type);
    }
}

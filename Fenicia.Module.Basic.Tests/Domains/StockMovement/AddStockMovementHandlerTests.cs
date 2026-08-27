using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class AddStockMovementHandlerTests : IDisposable
{
    private readonly TestCompanyContext companyContext;
    private readonly DefaultContext db;
    private readonly AddStockMovementHandler handler;

    public AddStockMovementHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddStockMovementHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsStockMovementAndReturnsResponse()
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
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.ProductId, result.ProductId);
        Assert.Equal(command.Quantity, result.Quantity);
        Assert.Equal(command.Type, result.Type);
    }

    [Fact]
    public async Task Handle_WithStockMovementIn_IncreasesProductQuantity()
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
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        await handler.Handle(command, CancellationToken.None);

        var updatedProduct = await db.BasicProducts.FindAsync([product.Id], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal(110, updatedProduct.Quantity);
    }

    [Fact]
    public async Task Handle_WithStockMovementOut_DecreasesProductQuantity()
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
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.Out, product.Id, null, null, null, null, null);

        await handler.Handle(command, CancellationToken.None);

        var updatedProduct = await db.BasicProducts.FindAsync([product.Id], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal(90, updatedProduct.Quantity);
    }

    [Fact]
    public async Task Handle_WithNullProduct_DoesNotUpdateQuantity()
    {

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, Guid.NewGuid(), null, null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        var movement = await db.BasicStockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(movement);
    }

    [Fact]
    public async Task Handle_VerifiesStockMovementWasSavedToDatabase()
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
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        await handler.Handle(command, CancellationToken.None);

        var movement = await db.BasicStockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(movement);
        Assert.Equal(command.Quantity, movement.Quantity);
        Assert.Equal(command.Type, movement.Type);
    }
}

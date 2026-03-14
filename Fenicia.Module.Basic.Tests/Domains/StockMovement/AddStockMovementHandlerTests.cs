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

        this.companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, this.companyContext);
        this.handler = new AddStockMovementHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsStockMovementAndReturnsResponse()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.ProductId, result.ProductId);
        Assert.Equal(command.Quantity, result.Quantity);
        Assert.Equal(command.Type, result.Type);
    }

    [Fact]
    public async Task Handle_WithStockMovementIn_IncreasesProductQuantity()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.db.BasicProducts.FindAsync([product.Id], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal(110, updatedProduct.Quantity);
    }

    [Fact]
    public async Task Handle_WithStockMovementOut_DecreasesProductQuantity()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.Out, product.Id, null, null, null, null, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.db.BasicProducts.FindAsync([product.Id], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal(90, updatedProduct.Quantity);
    }

    [Fact]
    public async Task Handle_WithNullProduct_DoesNotUpdateQuantity()
    {
        // Arrange
        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, Guid.NewGuid(), null, null, null, null, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var movement = await this.db.BasicStockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(movement);
    }

    [Fact]
    public async Task Handle_VerifiesStockMovementWasSavedToDatabase()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(Guid.NewGuid(), 10, DateTime.Now, 15.00m, StockMovementType.In, product.Id, null, null, null, null, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var movement = await this.db.BasicStockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(movement);
        Assert.Equal(command.Quantity, movement.Quantity);
        Assert.Equal(command.Type, movement.Type);
    }
}
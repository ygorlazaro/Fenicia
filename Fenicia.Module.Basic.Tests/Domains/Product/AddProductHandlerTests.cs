using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

/// <summary>
///     Unit tests for the AddProductHandler.
///     Tests product creation logic including validation and database operations.
/// </summary>
public class AddProductHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly AddProductHandler handler;

    public AddProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddProductHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProductAndReturnsResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new AddProductCommand(Guid.NewGuid(),
            "Product Name",
            "SKU001",
            "123456789",
            "Description",
            10.00m,
            20.00m,
            100,
            5,
            200,
            "http://image.com",
            1.5m,
            "10x10x10",
            "un",
            categoryId,
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal("SKU001", result.SKU);
        Assert.Equal("123456789", result.Barcode);
        Assert.Equal("Description", result.Description);
        Assert.Equal(command.CostPrice, result.CostPrice);
        Assert.Equal(command.SalesPrice, result.SalesPrice);
        Assert.Equal(command.Quantity, result.Quantity);
        Assert.Equal(5, result.MinStockLevel);
        Assert.Equal(200, result.MaxStockLevel);
        Assert.True(result.IsActive);
        Assert.Equal(command.CategoryId, result.CategoryId);
    }

    [Fact]
    public async Task Handle_VerifiesProductWasSavedToDatabase()
    {
        // Arrange
        var command = new AddProductCommand(Guid.NewGuid(),
            "Product Name",
            "SKU001",
            "123456789",
            "Description",
            10.00m,
            20.00m,
            100,
            5,
            200,
            "http://image.com",
            1.5m,
            "10x10x10",
            "un",
            Guid.NewGuid(),
            null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var product = await db.BasicProducts.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(product);
        Assert.Equal(command.Name, product.Name);
        Assert.Equal("SKU001", product.SKU);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProducts()
    {
        // Arrange
        var command1 = new AddProductCommand(Guid.NewGuid(), "Product 1", "SKU001", "123456789", "Description", 10.00m, 20.00m, 100, 5, 200, null, 1.5f, "10x10x10", "un", Guid.NewGuid(), null);

        var command2 = new AddProductCommand(Guid.NewGuid(), "Product 2", "SKU002", "987654321", "Description", 15.00m, 25.00m, 50, 10, 150, null, 2.0f, "20x20x20", "kg", Guid.NewGuid(), null);

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var products = await db.BasicProducts.ToListAsync();
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task Handle_WithNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var command = new AddProductCommand(Guid.NewGuid(), "Product Name", null, null, null, null, 20.00m, 100, null, null, null, null, null, null, Guid.NewGuid(), null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CostPrice);
    }
}

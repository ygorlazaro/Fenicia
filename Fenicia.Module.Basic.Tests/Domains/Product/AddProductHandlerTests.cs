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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddProductHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProductAndReturnsResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new AddProductCommand(Guid.NewGuid(), "Product Name", 10.00m, 20.00m, 100, categoryId, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.CostPrice, result.CostPrice);
        Assert.Equal(command.SalesPrice, result.SalesPrice);
        Assert.Equal(command.Quantity, result.Quantity);
        Assert.Equal(command.CategoryId, result.CategoryId);
    }

    [Fact]
    public async Task Handle_VerifiesProductWasSavedToDatabase()
    {
        // Arrange
        var command = new AddProductCommand(Guid.NewGuid(), "Product Name", 10.00m, 20.00m, 100, Guid.NewGuid(), null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var product = await this.db.BasicProducts.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(product);
        Assert.Equal(command.Name, product.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProducts()
    {
        // Arrange
        var command1 = new AddProductCommand(Guid.NewGuid(), "Product 1", 10.00m, 20.00m, 100, Guid.NewGuid(), null);

        var command2 = new AddProductCommand(Guid.NewGuid(), "Product 2", 15.00m, 25.00m, 50, Guid.NewGuid(), null);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var products = await this.db.BasicProducts.ToListAsync();
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task Handle_WithNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var command = new AddProductCommand(Guid.NewGuid(), "Product Name", null, 20.00m, 100, Guid.NewGuid(), null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.CostPrice);
    }
}
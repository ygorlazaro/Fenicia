using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class AddProductCategoryHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly AddProductCategoryHandler handler;

    public AddProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddProductCategoryHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsCategoryAndReturnsResponse()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), "Electronics");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryWasSavedToDatabase()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), "Books");

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var category = await db.BasicProductCategories.FindAsync([command.Id], CancellationToken.None);
        Assert.NotNull(category);
        Assert.Equal(command.Name, category.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllCategories()
    {
        // Arrange
        var command1 = new AddProductCategoryCommand(Guid.NewGuid(), "Electronics");
        var command2 = new AddProductCategoryCommand(Guid.NewGuid(), "Books");

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var categories = await db.BasicProductCategories.ToListAsync();
        Assert.Equal(2, categories.Count);
    }
}

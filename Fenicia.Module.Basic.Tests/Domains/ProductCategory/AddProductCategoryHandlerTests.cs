using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class AddProductCategoryHandlerTests : IDisposable
{
    public AddProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddProductCategoryHandler(this.db);
    }

    private readonly DefaultContext db;
    private readonly AddProductCategoryHandler handler;

    [Fact]
    public async Task Handle_WithValidCommand_AddsCategoryAndReturnsResponse()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), "Electronics");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

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
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var category = await this.db.BasicProductCategories.FindAsync([command.Id], CancellationToken.None);
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
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var categories = await this.db.BasicProductCategories.ToListAsync();
        Assert.Equal(2, categories.Count);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

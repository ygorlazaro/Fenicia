using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class AddProductCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddProductCategoryHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsCategoryAndReturnsResponse()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), "Electronics");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Name, Is.EqualTo(command.Name));
        }
    }

    [Test]
    public async Task Handle_VerifiesCategoryWasSavedToDatabase()
    {
        // Arrange
        var command = new AddProductCategoryCommand(Guid.NewGuid(), "Books");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var category = await this.context.ProductCategories.FindAsync([command.Id], CancellationToken.None);
        Assert.That(category, Is.Not.Null);
        Assert.That(category.Name, Is.EqualTo(command.Name));
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllCategories()
    {
        // Arrange
        var command1 = new AddProductCategoryCommand(Guid.NewGuid(), "Electronics");
        var command2 = new AddProductCategoryCommand(Guid.NewGuid(), "Books");

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var categories = await this.context.ProductCategories.ToListAsync();
        Assert.That(categories, Has.Count.EqualTo(2));
    }
}

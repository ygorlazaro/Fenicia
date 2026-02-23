using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class UpdateProductCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdateProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdateProductCategoryHandler handler = null!;

    [Test]
    public async Task Handle_WhenCategoryExists_UpdatesCategoryAndReturnsResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Old Category"
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(categoryId, "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(categoryId));
            Assert.That(result.Name, Is.EqualTo("New Category"));
        }
    }

    [Test]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesCategoryWasUpdatedInDatabase()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Old Category"
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(categoryId, "New Category");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCategory = await this.context.ProductCategories.FindAsync([categoryId], CancellationToken.None);
        Assert.That(updatedCategory, Is.Not.Null);
        Assert.That(updatedCategory.Name, Is.EqualTo("New Category"));
    }

    [Test]
    public async Task Handle_WithMultipleCategories_OnlyUpdatesSpecified()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };

        this.context.ProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(category1Id, "Home Appliances");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCategory1 = await this.context.ProductCategories.FindAsync([category1Id], CancellationToken.None);
        var notUpdatedCategory2 = await this.context.ProductCategories.FindAsync([category2Id], CancellationToken.None);

        Assert.That(updatedCategory1, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCategory1.Name, Is.EqualTo("Home Appliances"));
            Assert.That(notUpdatedCategory2, Is.Not.Null);
        }
        Assert.That(notUpdatedCategory2?.Name, Is.EqualTo("Books"));
    }
}

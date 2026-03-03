using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.ProductCategory.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class DeleteProductCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProductCategoryHandler handler = null!;

    [Test]
    public async Task Handle_WhenCategoryExists_SetsDeletedDate()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new BasicProductCategory
        {
            Id = categoryId,
            Name = "Electronics"
        };

        this.context.BasicProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(categoryId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCategory = await this.context.BasicProductCategories.FindAsync([categoryId], CancellationToken.None);
        Assert.That(deletedCategory, Is.Not.Null);
        Assert.That(deletedCategory.Deleted, Is.Not.Null);
        Assert.That(deletedCategory.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedCategory.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenCategoryDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var categories = await this.context.BasicProductCategories.ToListAsync();
        Assert.That(categories, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleCategories_OnlyDeletesSpecified()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new BasicProductCategory { Id = category1Id, Name = "Electronics" };
        var category2 = new BasicProductCategory { Id = category2Id, Name = "Books" };

        this.context.BasicProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(category1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCategory = await this.context.BasicProductCategories.FindAsync([category1Id], CancellationToken.None);
        var notDeletedCategory = await this.context.BasicProductCategories.FindAsync([category2Id], CancellationToken.None);

        Assert.That(deletedCategory, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedCategory.Deleted, Is.Not.Null);
            Assert.That(notDeletedCategory, Is.Not.Null);
        }
        Assert.That(notDeletedCategory?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var categories = await this.context.BasicProductCategories.ToListAsync();
        Assert.That(categories, Is.Empty);
    }
}

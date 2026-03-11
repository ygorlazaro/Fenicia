using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class DeleteProductCategoryHandlerTests : IDisposable
{
    public DeleteProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new DeleteProductCategoryHandler(this.db);
    }

    private readonly DefaultContext db;
    private readonly DeleteProductCategoryHandler handler;

    [Fact]
    public async Task Handle_WhenCategoryExists_SetsDeletedDate()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(categoryId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCategory = await this.db.BasicProductCategories.FindAsync([categoryId], CancellationToken.None);
        Assert.NotNull(deletedCategory);
        Assert.NotNull(deletedCategory.Deleted);
        Assert.True(deletedCategory.Deleted >= beforeDelete.AddSeconds(-1));
        Assert.True(deletedCategory.Deleted <= DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var categories = await this.db.BasicProductCategories.ToListAsync();
        Assert.Empty(categories);
    }

    [Fact]
    public async Task Handle_WithMultipleCategories_OnlyDeletesSpecified()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };

        this.db.BasicProductCategories.AddRange(category1, category2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(category1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCategory = await this.db.BasicProductCategories.FindAsync([category1Id], CancellationToken.None);
        var notDeletedCategory = await this.db.BasicProductCategories.FindAsync([category2Id], CancellationToken.None);

        Assert.NotNull(deletedCategory);
        Assert.NotNull(deletedCategory.Deleted);
        Assert.NotNull(notDeletedCategory);
        Assert.Null(notDeletedCategory.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var categories = await this.db.BasicProductCategories.ToListAsync();
        Assert.Empty(categories);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

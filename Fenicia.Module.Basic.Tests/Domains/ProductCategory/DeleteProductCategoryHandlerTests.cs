using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class DeleteProductCategoryHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly DeleteProductCategoryHandler handler;

    public DeleteProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProductCategoryHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_SetsDeletedDate()
    {

        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };

        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(categoryId);
        var beforeDelete = DateTime.Now;

        await handler.Handle(command, CancellationToken.None);

        var deletedCategory = await db.BasicProductCategories.FindAsync([categoryId], CancellationToken.None);
        Assert.NotNull(deletedCategory);
        Assert.NotNull(deletedCategory.Deleted);
        Assert.True(deletedCategory.Deleted >= beforeDelete.AddSeconds(-1));
        Assert.True(deletedCategory.Deleted <= DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_DoesNothing()
    {

        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var categories = await db.BasicProductCategories.ToListAsync();
        Assert.Empty(categories);
    }

    [Fact]
    public async Task Handle_WithMultipleCategories_OnlyDeletesSpecified()
    {

        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel
        {
            Id = category1Id,
            Name = "Electronics"
        };
        var category2 = new ProductCategoryModel
        {
            Id = category2Id,
            Name = "Books"
        };

        db.BasicProductCategories.AddRange(category1, category2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCategoryCommand(category1Id);

        await handler.Handle(command, CancellationToken.None);

        var deletedCategory = await db.BasicProductCategories.FindAsync([category1Id], CancellationToken.None);
        var notDeletedCategory = await db.BasicProductCategories.FindAsync([category2Id], CancellationToken.None);

        Assert.NotNull(deletedCategory);
        Assert.NotNull(deletedCategory.Deleted);
        Assert.NotNull(notDeletedCategory);
        Assert.Null(notDeletedCategory.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {

        var command = new DeleteProductCategoryCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var categories = await db.BasicProductCategories.ToListAsync();
        Assert.Empty(categories);
    }
}

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class UpdateProductCategoryHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly UpdateProductCategoryHandler handler;

    public UpdateProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new UpdateProductCategoryHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_UpdatesCategoryAndReturnsResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Old Category" };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(categoryId, "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("New Category", result.Name);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "New Category");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryWasUpdatedInDatabase()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Old Category" };

        this.db.BasicProductCategories.Add(category);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(categoryId, "New Category");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCategory = await this.db.BasicProductCategories.FindAsync([categoryId], CancellationToken.None);
        Assert.NotNull(updatedCategory);
        Assert.Equal("New Category", updatedCategory.Name);
    }

    [Fact]
    public async Task Handle_WithMultipleCategories_OnlyUpdatesSpecified()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };

        this.db.BasicProductCategories.AddRange(category1, category2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCategoryCommand(category1Id, "Home Appliances");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCategory1 = await this.db.BasicProductCategories.FindAsync([category1Id], CancellationToken.None);
        var notUpdatedCategory2 = await this.db.BasicProductCategories.FindAsync([category2Id], CancellationToken.None);

        Assert.NotNull(updatedCategory1);
        Assert.Equal("Home Appliances", updatedCategory1.Name);
        Assert.NotNull(notUpdatedCategory2);
        Assert.Equal("Books", notUpdatedCategory2.Name);
    }
}
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class GetProductCategoryByIdHandlerTests : IDisposable
{
    public GetProductCategoryByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetProductCategoryByIdHandler(this.db);
    }

    private readonly DefaultContext db;
    private readonly GetProductCategoryByIdHandler handler;

    [Fact]
    public async Task Handle_WhenCategoryExists_ReturnsCategoryResponse()
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

        var query = new GetProductCategoryByIdQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProductCategoryByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProductCategoryByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleCategories_ReturnsOnlyRequestedCategory()
    {
        // Arrange
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

        this.db.BasicProductCategories.AddRange(category1, category2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductCategoryByIdQuery(category1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category1Id, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

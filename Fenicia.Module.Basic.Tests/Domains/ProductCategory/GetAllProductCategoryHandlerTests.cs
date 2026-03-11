using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class GetAllProductCategoryHandlerTests : IDisposable
{
    public GetAllProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetAllProductCategoryHandler(this.db);
    }

    private readonly DefaultContext db;
    private readonly GetAllProductCategoryHandler handler;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WithCategories_ReturnsAllCategories()
    {
        // Arrange
        var category1 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Books" };

        this.db.BasicProductCategories.AddRange(category1, category2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Contains(result.Data, c => c.Id == category1.Id);
        Assert.Contains(result.Data, c => c.Id == category2.Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.db.BasicProductCategories.Add(category);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.db.BasicProductCategories.Add(category);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(5, result.Total);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.db.BasicProductCategories.Add(category);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

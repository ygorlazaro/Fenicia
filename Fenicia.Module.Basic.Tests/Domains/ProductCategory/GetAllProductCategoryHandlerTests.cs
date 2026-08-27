using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.Handlers;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class GetAllProductCategoryHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetAllProductCategoryHandler handler;

    public GetAllProductCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProductCategoryHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllProductCategoryQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WithCategories_ReturnsAllCategories()
    {

        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Books"
        };

        db.BasicProductCategories.AddRange(category1, category2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Contains(result.Data, c => c.Id == category1.Id);
        Assert.Contains(result.Data, c => c.Id == category2.Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        for (var i = 0; i < 25; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            db.BasicProductCategories.Add(category);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        for (var i = 0; i < 5; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            db.BasicProductCategories.Add(category);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(5, result.Total);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        for (var i = 0; i < 25; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            db.BasicProductCategories.Add(category);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }
}

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class GetAllProductCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetAllProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetAllProductCategoryHandler handler = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithCategories_ReturnsAllCategories()
    {
        // Arrange
        var category1 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Books" };

        this.context.ProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Any(c => c.Id == category1.Id));
            Assert.That(result.Any(c => c.Id == category2.Id));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCategories_ReturnsAllWithoutPagination()
    {
        // Arrange
        for (var i = 0; i < 20; i++)
        {
            var category = new ProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.context.ProductCategories.Add(category);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(20));
    }
}

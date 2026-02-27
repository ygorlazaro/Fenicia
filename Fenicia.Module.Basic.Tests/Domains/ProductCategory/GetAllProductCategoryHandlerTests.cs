using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.ProductCategory.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class GetAllProductCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options);
        this.handler = new GetAllProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private DefaultContext context = null!;
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
        var category1 = new BasicProductCategory { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new BasicProductCategory { Id = Guid.NewGuid(), Name = "Books" };

        this.context.BasicProductCategories.AddRange(category1, category2);
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
            var category = new BasicProductCategory
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.context.BasicProductCategories.Add(category);
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

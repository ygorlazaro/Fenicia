using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class GetProductCategoryByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetProductCategoryByIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetProductCategoryByIdHandler handler = null!;

    [Test]
    public async Task Handle_WhenCategoryExists_ReturnsCategoryResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };

        this.context.ProductCategories.Add(category);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductCategoryByIdQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(categoryId));
            Assert.That(result.Name, Is.EqualTo("Electronics"));
        }
    }

    [Test]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProductCategoryByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProductCategoryByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleCategories_ReturnsOnlyRequestedCategory()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };

        this.context.ProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductCategoryByIdQuery(category1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(category1Id));
            Assert.That(result.Name, Is.EqualTo("Electronics"));
        }
        Assert.That(result.Name, Is.Not.EqualTo("Books"));
    }
}

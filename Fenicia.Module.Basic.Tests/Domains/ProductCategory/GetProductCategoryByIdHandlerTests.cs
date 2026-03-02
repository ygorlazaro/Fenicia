using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.ProductCategory.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

[TestFixture]
public class GetProductCategoryByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProductCategoryByIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProductCategoryByIdHandler handler = null!;

    [Test]
    public async Task Handle_WhenCategoryExists_ReturnsCategoryResponse()
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

        var category1 = new BasicProductCategory { Id = category1Id, Name = "Electronics" };
        var category2 = new BasicProductCategory { Id = category2Id, Name = "Books" };

        this.context.BasicProductCategories.AddRange(category1, category2);
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

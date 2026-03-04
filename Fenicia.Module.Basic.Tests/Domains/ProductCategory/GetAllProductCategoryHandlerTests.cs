using Fenicia.Common.Data;
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

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProductCategoryHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
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
        Assert.That(result.Data, Is.Empty);
        Assert.That(result.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task Handle_WithCategories_ReturnsAllCategories()
    {
        // Arrange
        var category1 = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Books" };

        this.context.BasicProductCategories.AddRange(category1, category2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Has.Count.EqualTo(2));
        Assert.That(result.Total, Is.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data.Any(c => c.Id == category1.Id));
            Assert.That(result.Data.Any(c => c.Id == category2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var category = new BasicProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.context.BasicProductCategories.Add(category);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Has.Count.EqualTo(10));
        Assert.That(result.Total, Is.EqualTo(25));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        for (var i = 0; i < 5; i++)
        {
            var category = new BasicProductCategoryModel
            {
                Id = Guid.NewGuid(),
                Name = $"Category {i}"
            };
            this.context.BasicProductCategories.Add(category);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProductCategoryQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Empty);
        Assert.That(result.Total, Is.EqualTo(5));
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        for (var i = 0; i < 25; i++)
        {
            var category = new BasicProductCategoryModel
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
        Assert.That(result.Data, Has.Count.EqualTo(10));
        Assert.That(result.Total, Is.EqualTo(25));
    }
}

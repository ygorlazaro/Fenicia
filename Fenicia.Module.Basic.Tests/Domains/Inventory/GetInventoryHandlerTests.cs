using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Inventory.GetInventory;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

[TestFixture]
public class GetInventoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetInventoryHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetInventoryHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyInventory()
    {
        // Arrange
        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.TotalCostPrice, Is.EqualTo(0));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(0));
            Assert.That(result.TotalQuantity, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task Handle_WithProducts_ReturnsInventoryWithTotals()
    {
        // Arrange
        var category = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.BasicProductCategories.Add(category);

        var product1 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name =  this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items, Has.Count.EqualTo(2));
            Assert.That(result.TotalCostPrice, Is.EqualTo(25.00m));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(45.00m));
            Assert.That(result.TotalQuantity, Is.EqualTo(150));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var category = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.BasicProductCategories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            var product = new BasicProductModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Commerce.ProductName(),
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = category.Id
            };
            this.context.BasicProducts.Add(product);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithProductsOrderedByQuantity_ReturnsInAscendingOrder()
    {
        // Arrange
        var category = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.BasicProductCategories.Add(category);

        var product1 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        var product3 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category.Id
        };

        this.context.BasicProducts.AddRange(product1, product2, product3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items[0].Quantity, Is.LessThanOrEqualTo(result.Items[1].Quantity));
            Assert.That(result.Items[1].Quantity, Is.LessThanOrEqualTo(result.Items[2].Quantity));
        }
    }

    [Test]
    public async Task Handle_VerifiesCategoryNameIsIncluded()
    {
        // Arrange
        var category = new BasicProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.BasicProductCategories.Add(category);

        var product = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].CategoryName, Is.EqualTo("Electronics"));
    }
}

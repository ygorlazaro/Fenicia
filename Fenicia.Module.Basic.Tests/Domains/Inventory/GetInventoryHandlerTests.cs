using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryHandlerTests : IDisposable
{
    public GetInventoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetInventoryHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly GetInventoryHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyInventory()
    {
        // Arrange
        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0,
            result.TotalCostPrice);
        Assert.Equal(0,
            result.TotalSalesPrice);
        Assert.Equal(0,
            result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithProducts_ReturnsInventoryWithTotals()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name =  this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        this.db.BasicProducts.AddRange(product1,
            product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Items.Count);
        Assert.Equal(25.00m,
            result.TotalCostPrice);
        Assert.Equal(45.00m,
            result.TotalSalesPrice);
        Assert.Equal(150,
            result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            var product = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Commerce.ProductName(),
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = category.Id
            };
            this.db.BasicProducts.Add(product);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery(2);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Items.Count);
    }

    [Fact]
    public async Task Handle_WithProductsOrderedByQuantity_ReturnsInAscendingOrder()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        var product3 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category.Id
        };

        this.db.BasicProducts.AddRange(product1,
            product2,
            product3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items[0].Quantity <= result.Items[1].Quantity);
        Assert.True(result.Items[1].Quantity <= result.Items[2].Quantity);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryNameIsIncluded()
    {
        // Arrange
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Electronics",
            result.Items[0].CategoryName);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

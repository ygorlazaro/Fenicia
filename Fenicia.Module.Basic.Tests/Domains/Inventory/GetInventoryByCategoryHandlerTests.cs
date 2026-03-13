using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryByCategoryHandlerTests : IDisposable
{
    public GetInventoryByCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetInventoryByCategoryHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly GetInventoryByCategoryHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithNoProductsForCategory_ReturnsEmptyInventory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetInventoryByCategoryQuery(categoryId);

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
    public async Task Handle_WithProductsForCategory_ReturnsFilteredInventory()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };
        this.db.BasicProductCategories.AddRange(category1,
            category2);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category1Id
        };

        var product3 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category2Id
        };

        this.db.BasicProducts.AddRange(product1,
            product2,
            product3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(category1Id);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Items.Count);
        Assert.True(result.Items.All(i => i.CategoryName == "Electronics"));
    }

    [Fact]
    public async Task Handle_WithProductsForCategory_CalculatesCorrectTotals()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = categoryId
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = categoryId
        };

        this.db.BasicProducts.AddRange(product1,
            product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
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
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
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
                CategoryId = categoryId
            };
            this.db.BasicProducts.Add(product);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId,
            2);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Items.Count);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }
}

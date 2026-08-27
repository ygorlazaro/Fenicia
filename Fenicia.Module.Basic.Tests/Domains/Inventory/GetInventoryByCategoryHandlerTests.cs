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
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetInventoryByCategoryHandler handler;

    public GetInventoryByCategoryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetInventoryByCategoryHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithNoProductsForCategory_ReturnsEmptyInventory()
    {

        var categoryId = Guid.NewGuid();
        var query = new GetInventoryByCategoryQuery(categoryId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCostPrice);
        Assert.Equal(0, result.TotalSalesPrice);
        Assert.Equal(0, result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithProductsForCategory_ReturnsFilteredInventory()
    {

        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel
        {
            Id = category1Id,
            Name = "Electronics"
        };
        var category2 = new ProductCategoryModel
        {
            Id = category2Id,
            Name = "Books"
        };
        db.BasicProductCategories.AddRange(category1, category2);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category1Id
        };

        var product3 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category2Id
        };

        db.BasicProducts.AddRange(product1, product2, product3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(category1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.True(result.Items.All(i => i.CategoryName == "Electronics"));
    }

    [Fact]
    public async Task Handle_WithProductsForCategory_CalculatesCorrectTotals()
    {

        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = categoryId
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = categoryId
        };

        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(25.00m, result.TotalCostPrice);
        Assert.Equal(45.00m, result.TotalSalesPrice);
        Assert.Equal(150, result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            var product = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Commerce.ProductName(),
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = categoryId
            };
            db.BasicProducts.Add(product);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId, 2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
    }
}

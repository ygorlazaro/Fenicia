using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryByProductHandlerTests : IDisposable
{
    public GetInventoryByProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetInventoryByProductHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly GetInventoryByProductHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithNonExistentProduct_ReturnsEmptyInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryByProductQuery(productId);

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
    public async Task Handle_WithExistingProduct_ReturnsProductInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(productId,
            result.Items[0].Id);
        Assert.Equal(product.Name,
            result.Items[0].Name);
        Assert.Equal(10.00m,
            result.TotalCostPrice);
        Assert.Equal(20.00m,
            result.TotalSalesPrice);
        Assert.Equal(100,
            result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithProductHavingNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = null,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(0,
            result.TotalCostPrice);
        Assert.Equal(20.00m,
            result.TotalSalesPrice);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryNameIsIncluded()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

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

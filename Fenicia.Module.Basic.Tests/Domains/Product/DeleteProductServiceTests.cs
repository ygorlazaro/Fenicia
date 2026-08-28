using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class DeleteProductServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductService service;

    public DeleteProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProductService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_SetsDeletedDate()
    {
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = faker.Random.Int(1, 100),
            CategoryId = Guid.NewGuid(),
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteProductCommand(product.Id), CancellationToken.None);

        var updated = await db.BasicProducts.FindAsync(product.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeleteProductCommand(Guid.NewGuid()), CancellationToken.None);

        var count = await db.BasicProducts.CountAsync();
        Assert.Equal(0, count);
    }
}

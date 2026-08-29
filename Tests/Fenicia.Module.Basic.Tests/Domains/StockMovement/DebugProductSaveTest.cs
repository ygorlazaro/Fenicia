using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class DebugProductSaveTest : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    public DebugProductSaveTest()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DebugSaveAndRead()
    {
        var product = new ProductModel { Id = Guid.NewGuid(), Name = _faker.Commerce.ProductName(), SalesPrice = 100, Quantity = 10, CategoryId = Guid.NewGuid() };
        _db.BasicProducts.Add(product);
        await _db.SaveChangesAsync(CancellationToken.None);

        var all = await _db.BasicProducts.ToListAsync();
        Assert.Single(all);

        var found = await _db.BasicProducts.FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(found);
        Assert.Equal(10, found.Quantity);
    }
}

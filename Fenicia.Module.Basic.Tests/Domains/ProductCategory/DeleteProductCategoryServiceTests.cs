using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class DeleteProductCategoryServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly ProductCategoryService service;

    public DeleteProductCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var productCategoryRepository = new ProductCategoryRepository(db);
        service = new ProductCategoryService(productCategoryRepository);
        faker = new Faker();
        var companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryExists_SetsDeletedDate()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteProductCategoryCommand(category.Id), companyId, CancellationToken.None);

        var updated = await db.BasicProductCategories.FindAsync(category.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenCategoryDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeleteProductCategoryCommand(Guid.NewGuid()), companyId, CancellationToken.None);

        var count = await db.BasicProductCategories.CountAsync();
        Assert.Equal(0, count);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Equal(0, count);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
        await db.SaveChangesAsync(CancellationToken.None);
        await service.DeleteAsync(new DeleteProductCategoryCommand(category.Id), companyId, CancellationToken.None);
        await service.DeleteAsync(new DeleteProductCategoryCommand(Guid.NewGuid()), companyId, CancellationToken.None);
        db.BasicProductCategories.Add(category);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductCategoryService service;
    public async Task DeleteAsync_WhenCategoryDoesNotExist_DoesNothing()
    public async Task DeleteAsync_WhenCategoryExists_SetsDeletedDate()
public class DeleteProductCategoryServiceTests : IDisposable
    public DeleteProductCategoryServiceTests()
    public void Dispose()
        service = new ProductCategoryService(productCategoryRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var count = await db.BasicProductCategories.CountAsync();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productCategoryRepository = new ProductCategoryRepository(db);
        var updated = await db.BasicProductCategories.FindAsync(category.Id);

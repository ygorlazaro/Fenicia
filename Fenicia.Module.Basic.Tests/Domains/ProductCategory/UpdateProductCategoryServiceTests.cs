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
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(newName, result.Name);
        Assert.NotNull(result);
        Assert.Null(result);
        await db.SaveChangesAsync(CancellationToken.None);
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
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNull()
    public async Task UpdateAsync_WhenCategoryExists_ReturnsUpdateResponse()
public class UpdateProductCategoryServiceTests : IDisposable
    public UpdateProductCategoryServiceTests()
    public void Dispose()
        service = new ProductCategoryService(productCategoryRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new UpdateProductCategoryCommand(category.Id, newName);
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var newName = faker.Commerce.Categories(1).First();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productCategoryRepository = new ProductCategoryRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

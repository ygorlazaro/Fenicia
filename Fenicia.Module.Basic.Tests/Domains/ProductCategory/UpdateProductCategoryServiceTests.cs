using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class UpdateProductCategoryServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductCategoryService service;

    public UpdateProductCategoryServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProductCategoryService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryExists_ReturnsUpdateResponse()
    {
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var newName = faker.Commerce.Categories(1).First();
        var command = new UpdateProductCategoryCommand(category.Id, newName);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(newName, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ReturnsNull()
    {
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.Null(result);
    }
}

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
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.NotNull(result);
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
    public AddProductCategoryServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddProductCategoryResponse()
public class AddProductCategoryServiceTests : IDisposable
    public void Dispose()
        service = new ProductCategoryService(productCategoryRepository);
        var command = new AddProductCategoryCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productCategoryRepository = new ProductCategoryRepository(db);
        var result = await service.AddAsync(command, companyId, CancellationToken.None);

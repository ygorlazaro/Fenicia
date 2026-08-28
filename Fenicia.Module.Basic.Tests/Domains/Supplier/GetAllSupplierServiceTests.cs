using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Total);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        await db.SaveChangesAsync(CancellationToken.None);
            Cnpj = faker.Random.Replace("##.###.###/####-##")
        db.BasicSuppliers.Add(supplier);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Supplier;
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####")
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;
    public async Task GetAllAsync_WhenNoSuppliers_ReturnsEmptyPagination()
    public async Task GetAllAsync_WhenSuppliersExist_ReturnsPaginationWithSuppliers()
public class GetAllSupplierServiceTests : IDisposable
    public GetAllSupplierServiceTests()
    public void Dispose()
        service = new SupplierService(supplierRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var person = new PersonModel
        var result = await service.GetAllAsync(new GetAllSupplierQuery(1, 10), CancellationToken.None);
        var supplier = new SupplierModel
        var supplierRepository = new SupplierRepository(db);

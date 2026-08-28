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
        Assert.Equal(supplier.Id, result.Id);
        Assert.NotNull(result);
        Assert.Null(result);
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
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
public class GetSupplierByIdServiceTests : IDisposable
    public GetSupplierByIdServiceTests()
    public void Dispose()
        service = new SupplierService(supplierRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var person = new PersonModel
        var result = await service.GetByIdAsync(new GetSupplierByIdQuery(Guid.NewGuid()), CancellationToken.None);
        var result = await service.GetByIdAsync(new GetSupplierByIdQuery(supplier.Id), CancellationToken.None);
        var supplier = new SupplierModel
        var supplierRepository = new SupplierRepository(db);

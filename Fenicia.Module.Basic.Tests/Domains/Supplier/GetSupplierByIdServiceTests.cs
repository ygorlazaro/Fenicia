using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class GetSupplierByIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly SupplierService service;

    public GetSupplierByIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var supplierRepository = new SupplierRepository(db);
        service = new SupplierService(supplierRepository);
        faker = new Faker();
        var companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierExists_ReturnsSupplier()
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            Document = faker.Random.Replace("###.###.###-##"),
            PhoneNumber = faker.Random.Replace("(##) #####-####")
        };

        var supplier = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Person = person,
            PersonId = person.Id,
            Cnpj = faker.Random.Replace("##.###.###/####-##")
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByIdAsync(new GetSupplierByIdQuery(supplier.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(supplier.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var result = await service.GetByIdAsync(new GetSupplierByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }
}

using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.DataSource;

public class GetAllSupplierForDataSourceServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DataSourceService service;

    public GetAllSupplierForDataSourceServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new DataSourceService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetSuppliersAsync_WhenNoSuppliers_ReturnsEmptyList()
    {
        var result = await service.GetSuppliersAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetSuppliersAsync_WhenSuppliersExist_ReturnsSuppliers()
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

        var result = await service.GetSuppliersAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}

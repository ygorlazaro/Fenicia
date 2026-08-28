using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
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
namespace Fenicia.Module.Basic.Tests.Domains.DataSource;
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####")
    private readonly DataSourceService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    public async Task GetSuppliersAsync_WhenNoSuppliers_ReturnsEmptyList()
    public async Task GetSuppliersAsync_WhenSuppliersExist_ReturnsSuppliers()
public class GetAllSupplierForDataSourceServiceTests : IDisposable
    public GetAllSupplierForDataSourceServiceTests()
    public void Dispose()
        service = new DataSourceService(db);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var person = new PersonModel
        var result = await service.GetSuppliersAsync(CancellationToken.None);
        var supplier = new SupplierModel

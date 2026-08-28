using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Equal(command.Cnpj, result.Cnpj);
        Assert.Equal(command.Id, result.Id);
        Assert.NotNull(result);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
            faker.Internet.Email(),
        faker = new Faker();
            faker.Person.FullName,
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("##.###.###/####-##"),
            faker.Random.Replace("(##) #####-####"),
        GC.SuppressFinalize(this);
            Guid.NewGuid(),
namespace Fenicia.Module.Basic.Tests.Domains.Supplier;
            null);
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;
    public AddSupplierServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddSupplierResponse()
public class AddSupplierServiceTests : IDisposable
    public void Dispose()
        service = new SupplierService(supplierRepository);
        var command = new AddSupplierCommand(
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await service.AddAsync(command, companyId, CancellationToken.None);
        var supplierRepository = new SupplierRepository(db);

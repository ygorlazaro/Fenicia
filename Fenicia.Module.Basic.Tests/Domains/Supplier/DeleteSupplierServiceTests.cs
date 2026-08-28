using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

            {
            },
        {
        };
    {
    }
{
}
        Assert.Equal(0, count);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
        await db.SaveChangesAsync(CancellationToken.None);
        await service.DeleteAsync(new DeleteSupplierCommand(Guid.NewGuid()), companyId, CancellationToken.None);
        await service.DeleteAsync(new DeleteSupplierCommand(supplier.Id), companyId, CancellationToken.None);
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
            Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Supplier;
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
                PhoneNumber = faker.Random.Replace("(##) #####-####")
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;
    public async Task DeleteAsync_WhenSupplierDoesNotExist_DoesNothing()
    public async Task DeleteAsync_WhenSupplierExists_SetsDeletedDate()
public class DeleteSupplierServiceTests : IDisposable
    public DeleteSupplierServiceTests()
    public void Dispose()
        service = new SupplierService(supplierRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var count = await db.BasicSuppliers.CountAsync();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var supplier = new SupplierModel
        var supplierRepository = new SupplierRepository(db);
        var updated = await db.BasicSuppliers.FindAsync(supplier.Id);

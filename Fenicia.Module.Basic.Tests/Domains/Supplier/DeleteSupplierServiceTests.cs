using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class DeleteSupplierServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly SupplierService service;

    public DeleteSupplierServiceTests()
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
    public async Task DeleteAsync_WhenSupplierExists_SetsDeletedDate()
    {
        var supplier = new SupplierModel
        {
            Id = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####")
            },
            PersonId = Guid.NewGuid(),
            Cnpj = faker.Random.Replace("##.###.###/####-##")
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteSupplierCommand(supplier.Id), companyId, CancellationToken.None);

        var updated = await db.BasicSuppliers.FindAsync(supplier.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenSupplierDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeleteSupplierCommand(Guid.NewGuid()), companyId, CancellationToken.None);

        var count = await db.BasicSuppliers.CountAsync();
        Assert.Equal(0, count);
    }
}

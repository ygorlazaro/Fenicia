using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class UpdateSupplierServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;

    public UpdateSupplierServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new SupplierService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierExists_ReturnsUpdateResponse()
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

        var newName = faker.Person.FullName;
        var command = new UpdateSupplierCommand(supplier.Id, newName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), supplier.Cnpj, null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(supplier.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenSupplierDoesNotExist_ReturnsNull()
    {
        var command = new UpdateSupplierCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), faker.Random.Replace("##.###.###/####-##"), null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.Null(result);
    }
}

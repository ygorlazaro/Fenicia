using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Common;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class UpdateSupplierHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateSupplierHandler handler;

    public UpdateSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateSupplierHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenSupplierExists_UpdatesSupplierAndReturnsResponse()
    {

        var supplierId = Guid.NewGuid();
        var supplier = new SupplierModel
        {
            Id = supplierId,
            Cnpj = "12.345.678/0001-90",
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com"
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "11988887777",
            "98.765.432/0001-10",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("98.765.432/0001-10", result.Cnpj);
    }

    [Fact]
    public async Task Handle_WhenSupplierDoesNotExist_ReturnsNull()
    {

        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            null,
            null,
            null,
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var command = new UpdateSupplierCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            null,
            null,
            null,
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesSupplierWasUpdatedInDatabase()
    {

        var supplierId = Guid.NewGuid();
        var supplier = new SupplierModel
        {
            Id = supplierId,
            Cnpj = "12.345.678/0001-90",
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com"
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            null,
            null,
            "98.765.432/0001-10",
            null);

        await handler.Handle(command, CancellationToken.None);

        var updatedSupplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == supplierId);

        Assert.NotNull(updatedSupplier);
        Assert.Equal("New Name", updatedSupplier.Person.Name);
        Assert.Equal("98.765.432/0001-10", updatedSupplier.Cnpj);
    }

    [Fact]
    public async Task Handle_WithAddress_CreatesOrUpdatesAddress()
    {

        var stateId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = stateId,
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        var supplierId = Guid.NewGuid();
        var supplier = new SupplierModel
        {
            Id = supplierId,
            Cnpj = "12.345.678/0001-90",
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com"
            }
        };

        db.BasicSuppliers.Add(supplier);
        await db.SaveChangesAsync(CancellationToken.None);

        var addressDto = new AddressDTO(
            faker.Address.StreetName(),
            faker.Random.Replace("####"),
            "Apt 202",
            faker.Address.CityPrefix(),
            faker.Address.ZipCode(),
            stateId,
            faker.Address.City(),
            "Brasil"
        );

        var command = new UpdateSupplierCommand(
            supplierId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "11988887777",
            "98.765.432/0001-10",
            addressDto);

        await handler.Handle(command, CancellationToken.None);

        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}

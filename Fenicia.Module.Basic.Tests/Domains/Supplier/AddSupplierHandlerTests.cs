using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Common;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class AddSupplierHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddSupplierHandler handler;

    public AddSupplierHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddSupplierHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsSupplierAndReturnsResponse()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(), 
            faker.Company.CompanyName(), 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            faker.Random.Replace("(##) #####-####"), 
            faker.Random.Replace("##.###.###/####-##"), 
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Cnpj, result.Cnpj);
    }

    [Fact]
    public async Task Handle_VerifiesSupplierWasSavedToDatabase()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(), 
            faker.Company.CompanyName(), 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            null, 
            null, 
            null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.NotNull(supplier);
        Assert.Equal(command.Name, supplier.Person.Name);
    }

    [Fact]
    public async Task Handle_WithNullCnpj_HandlesCorrectly()
    {
        // Arrange
        var command = new AddSupplierCommand(
            Guid.NewGuid(), 
            faker.Company.CompanyName(), 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            null, 
            null, 
            null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Cnpj);
    }

    [Fact]
    public async Task Handle_WithAddress_CreatesAddressAndPersonAddressRelationship()
    {
        // Arrange
        var stateId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = stateId,
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);
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

        var command = new AddSupplierCommand(
            Guid.NewGuid(), 
            faker.Company.CompanyName(), 
            faker.Internet.Email(), 
            faker.Random.Replace("###.###.###-##"), 
            faker.Random.Replace("(##) #####-####"), 
            faker.Random.Replace("##.###.###/####-##"), 
            addressDto);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}

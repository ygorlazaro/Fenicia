using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Common;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class AddCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddCustomerHandler handler;

    public AddCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddCustomerHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsResponse()
    {

        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.Id });
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {

        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            null,
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_VerifiesCustomerWasSavedToDatabase()
    {

        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        var customer = await db.BasicCustomers.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == result.Id);

        Assert.NotNull(customer);
        Assert.Equal(command.Name, customer.Person.Name);
        Assert.Equal(command.Email, customer.Person.Email);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllCustomers()
    {

        var command1 = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            null,
            null);

        var command2 = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            null,
            null);

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var customers = await db.BasicCustomers.ToListAsync();
        Assert.Equal(2, customers.Count);
    }

    [Fact]
    public async Task Handle_WithAddress_CreatesAddressAndPersonAddressRelationship()
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

        var addressDto = new AddressCommand(
            faker.Address.StreetName(),
            faker.Random.Replace("####"),
            "Apt 101",
            faker.Address.CityPrefix(),
            faker.Address.ZipCode(),
            stateId,
            faker.Address.City(),
            "Brasil"
        );

        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            addressDto);

        await handler.Handle(command, CancellationToken.None);

        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}

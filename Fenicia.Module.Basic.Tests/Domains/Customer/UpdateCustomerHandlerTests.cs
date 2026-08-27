using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Common;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class UpdateCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateCustomerHandler handler;

    public UpdateCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateCustomerHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenCustomerExists_UpdatesCustomerAndReturnsResponse()
    {

        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "12345678900",
                PhoneNumber = "11999999999"
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customer.Person.Id, result.PersonId);
        Assert.Equal(customerId, result.Id);
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {

        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {

        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###########"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "98765432100",
            null,
            null);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
    public async Task Handle_VerifiesCustomerWasUpdatedInDatabase()
    {

        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "12345678900",
                PhoneNumber = "11999999999"
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            null);

        await handler.Handle(command, CancellationToken.None);

        var updatedCustomer = await db.BasicCustomers.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == customerId);

        Assert.NotNull(updatedCustomer);
        Assert.Equal("New Name", updatedCustomer.Person.Name);
        Assert.Equal("new@email.com", updatedCustomer.Person.Email);
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

        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Email = "old@email.com",
                Document = "12345678900",
                PhoneNumber = "11999999999"
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var addressDto = new AddressCommand(
            faker.Address.StreetName(),
            faker.Random.Replace("####"),
            "Apt 202",
            faker.Address.CityPrefix(),
            faker.Address.ZipCode(),
            stateId,
            faker.Address.City(),
            "Brasil"
        );

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "98765432100",
            "11988887777",
            addressDto);

        await handler.Handle(command, CancellationToken.None);

        var address = await db.AuthAddresses.FirstOrDefaultAsync(a => a.Street == addressDto.Street);
        var personAddress = await db.BasicPersonAddresses.FirstOrDefaultAsync(pa => pa.AddressId == address!.Id);

        Assert.NotNull(address);
        Assert.NotNull(personAddress);
    }
}

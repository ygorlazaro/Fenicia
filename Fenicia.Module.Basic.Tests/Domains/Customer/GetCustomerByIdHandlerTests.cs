using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Handlers;
using Fenicia.Module.Basic.Domains.Customer.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class GetCustomerByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetCustomerByIdHandler handler;

    public GetCustomerByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetCustomerByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenCustomerExists_ReturnsCustomerResponse()
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
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(customer.Person.Id, result.PersonId);
        Assert.Equal(customer.Person.Name, result.Name);
        Assert.Equal(customer.Person.Email, result.Email);
        Assert.Equal(customer.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(customer.Person.Document, result.Document);
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {

        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleCustomers_ReturnsOnlyRequestedCustomer()
    {

        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var customer1 = new CustomerModel
        {
            Id = customer1Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = customer2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FirstName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customer1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customer1Id, result.Id);
        Assert.Equal(customer1.Person.Id, result.PersonId);
        Assert.Equal(customer1.Person.Name, result.Name);
    }

    [Fact]
    public async Task Handle_WithAddress_ReturnsFullAddressDetails()
    {

        var customerId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var personId = Guid.NewGuid();

        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

        var address = new AddressModel
        {
            Id = addressId,
            Street = faker.Address.StreetName(),
            Number = faker.Random.Replace("####"),
            ZipCode = faker.Address.ZipCode(),
            StateId = state.Id,
            City = faker.Address.City()
        };
        db.AuthAddresses.Add(address);

        var customer = new CustomerModel
        {
            Id = customerId,
            PersonId = personId,
            Person = new PersonModel
            {
                Id = personId,
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber(),
                PersonAddresses = new List<PersonAddressModel>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        PersonId = personId,
                        AddressId = addressId
                    }
                }
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Address);
        Assert.Equal(addressId, result.Address.Id);
        Assert.Equal(address.Street, result.Address.Street);
    }
}

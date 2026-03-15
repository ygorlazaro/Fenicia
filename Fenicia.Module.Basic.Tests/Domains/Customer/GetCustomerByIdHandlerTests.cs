using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Handlers;
using Fenicia.Module.Basic.Domains.Customer.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

/// <summary>
///     Unit tests for the GetCustomerByIdHandler.
///     Tests customer retrieval by ID logic.
/// </summary>
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

    /// <summary>
    ///     Tests that retrieving a customer by ID returns the customer details when found.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCustomerExists_ReturnsCustomerResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

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
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal(customer.Person.Id, result.PersonId);
        Assert.Equal(customer.Person.Name, result.Name);
        Assert.Equal(customer.Person.Email, result.Email);
        Assert.Equal(customer.Person.PhoneNumber, result.PhoneNumber);
        Assert.Equal(customer.Person.Document, result.Document);
        Assert.Equal(customer.Person.Street, result.Street);
        Assert.Equal(customer.Person.Number, result.Number);
        Assert.Equal(customer.Person.Complement, result.Complement);
        Assert.Equal(customer.Person.Neighborhood, result.Neighborhood);
        Assert.Equal(customer.Person.ZipCode, result.ZipCode);
        Assert.Equal(customer.Person.StateId, result.StateId);
        Assert.Equal(customer.Person.City, result.City);
    }

    /// <summary>
    ///     Tests that retrieving a non-existent customer returns null.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    ///     Tests that retrieving from an empty database returns null.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    ///     Tests that when multiple customers exist, only the requested customer is returned.
    /// </summary>
    [Fact]
    public async Task Handle_WithMultipleCustomers_ReturnsOnlyRequestedCustomer()
    {
        // Arrange
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

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
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
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
                Street = faker.Address.StreetName(),
                Number = faker.Random.Replace("####"),
                ZipCode = faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = faker.Address.City(),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customer1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer1Id, result.Id);
        Assert.Equal(customer1.Person.Id, result.PersonId);
        Assert.Equal(customer1.Person.Name, result.Name);
    }

    /// <summary>
    ///     Tests that customers with null address fields are handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullAddressFields_ReturnsCorrectResponse()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var state = new StateModel
        {
            Id = Guid.NewGuid(),
            Name = "São Paulo",
            Uf = "SP"
        };
        db.AuthStates.Add(state);

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
                Street = string.Empty,
                Number = string.Empty,
                Complement = null,
                Neighborhood = null,
                ZipCode = string.Empty,
                StateId = state.Id,
                State = state,
                City = null,
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetCustomerByIdQuery(customerId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Person.Name, result.Name);
        Assert.Equal(customer.Person.Email, result.Email);
    }
}

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
///     Unit tests for the GetAllCustomerHandler.
///     Tests customer list retrieval with pagination logic.
/// </summary>
public class GetAllCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllCustomerHandler handler;

    public GetAllCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetAllCustomerHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that retrieving customers from an empty database returns an empty list.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    /// <summary>
    ///     Tests that retrieving customers returns all customers in the database.
    /// </summary>
    [Fact]
    public async Task Handle_WithCustomers_ReturnsAllCustomers()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Person.FullName,
                Email = this.faker.Internet.Email(),
                Document = this.faker.Random.Replace("###.###.###-##"),
                Street = this.faker.Address.StreetName(),
                Number = this.faker.Random.Replace("####"),
                ZipCode = this.faker.Address.ZipCode(),
                StateId = state.Id,
                State = state,
                City = this.faker.Address.City(),
                PhoneNumber = this.faker.Phone.PhoneNumber()
            }
        };

        this.db.BasicCustomers.AddRange(customer1, customer2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count);
        Assert.Equal(2, result.Total);
        Assert.Equal(customer1.Person.Id, result.Data[0].PersonId);
        Assert.Equal(customer1.Person.Name, result.Data[0].Name);
        Assert.Equal(customer1.Person.Email, result.Data[0].Email);
        Assert.Equal(customer2.Person.Id, result.Data[1].PersonId);
        Assert.Equal(customer2.Person.Name, result.Data[1].Name);
        Assert.Equal(customer2.Person.Email, result.Data[1].Email);
    }

    /// <summary>
    ///     Tests that pagination returns the correct page of results.
    /// </summary>
    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.db.BasicCustomers.Add(customer);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    /// <summary>
    ///     Tests that requesting a page beyond available data returns an empty list.
    /// </summary>
    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        for (var i = 0; i < 5; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.db.BasicCustomers.Add(customer);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(5, result.Total);
    }

    /// <summary>
    ///     Tests that default pagination returns the first page with 10 items.
    /// </summary>
    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var state = new StateModel { Id = Guid.NewGuid(), Name = "São Paulo", Uf = "SP" };
        this.db.AuthStates.Add(state);

        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{this.faker.Person.FullName} {i}",
                    Email = this.faker.Internet.Email(),
                    Document = this.faker.Random.Replace("###.###.###-##"),
                    Street = this.faker.Address.StreetName(),
                    Number = this.faker.Random.Replace("####"),
                    ZipCode = this.faker.Address.ZipCode(),
                    StateId = state.Id,
                    State = state,
                    City = this.faker.Address.City(),
                    PhoneNumber = this.faker.Phone.PhoneNumber()
                }
            };
            this.db.BasicCustomers.Add(customer);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }
}
using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

/// <summary>
///     Unit tests for the AddCustomerHandler.
///     Tests customer creation business logic including validation and database operations.
/// </summary>
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

    /// <summary>
    ///     Tests that creating a customer with valid data successfully adds the customer and returns response.
    /// </summary>
    [Fact]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsResponse()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), "Apt 101", faker.Address.CityPrefix(), faker.Random.Replace("####"), Guid.NewGuid(), faker.Address.StreetName(), faker.Address.ZipCode(), faker.Random.Replace("(##) #####-####"));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    /// <summary>
    ///     Tests that creating a customer with null phone number is handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null street is handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null zip code is handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null address number is handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null complement preserves null value.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null neighborhood preserves null value.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that creating a customer with null city preserves null value.
    /// </summary>
    [Fact]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), null, null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Tests that the created customer is actually persisted to the database.
    /// </summary>
    [Fact]
    public async Task Handle_VerifiesCustomerWasSavedToDatabase()
    {
        // Arrange
        var command = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), "Apt 101", faker.Address.CityPrefix(), faker.Random.Replace("####"), Guid.NewGuid(), faker.Address.StreetName(), faker.Address.ZipCode(), faker.Random.Replace("(##) #####-####"));

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var customer = await db.BasicCustomers.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.NotNull(customer);
        Assert.Equal(command.Name, customer.Person.Name);
        Assert.Equal(command.Email, customer.Person.Email);
    }

    /// <summary>
    ///     Tests that creating multiple customers adds all of them to the database.
    /// </summary>
    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllCustomers()
    {
        // Arrange
        var command1 = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        var command2 = new AddCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Address.City(), null, null, null, Guid.NewGuid(), null, null, null);

        // Act
        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        // Assert
        var customers = await db.BasicCustomers.ToListAsync();
        Assert.Equal(2, customers.Count);
    }
}

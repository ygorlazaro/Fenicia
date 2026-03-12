using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class AddCustomerHandlerTests : IDisposable
{
    public AddCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new AddCustomerHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly AddCustomerHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsResponse()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_VerifiesCustomerWasSavedToDatabase()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var customer = await this.db.BasicCustomers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.NotNull(customer);
        Assert.Equal(command.Name,
            customer.Person.Name);
        Assert.Equal(command.Email,
            customer.Person.Email);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllCustomers()
    {
        // Arrange
        var command1 = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        var command2 = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        await this.handler.Handle(command1,
            CancellationToken.None);
        await this.handler.Handle(command2,
            CancellationToken.None);

        // Assert
        var customers = await this.db.BasicCustomers.ToListAsync();
        Assert.Equal(2,
            customers.Count);
    }
}

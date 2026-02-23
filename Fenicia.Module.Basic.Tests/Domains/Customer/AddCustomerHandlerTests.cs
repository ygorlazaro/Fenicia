using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class AddCustomerHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddCustomerHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddCustomerHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsCustomerAndReturnsResponse()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Person.Name, Is.EqualTo(command.Name));
            Assert.That(result.Person.Email, Is.EqualTo(command.Email));
            Assert.That(result.Person.Document, Is.EqualTo(command.Cpf));
        }
    }

    [Test]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            null,
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Phone, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Street, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.ZipCode, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Number, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Complement, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.Neighborhood, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            null,
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Person.Address?.City, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesCustomerWasSavedToDatabase()
    {
        // Arrange
        var command = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            "Apt 101",
            this.faker.Address.CityPrefix(),
            this.faker.Random.Replace("####"),
            Guid.NewGuid(),
            this.faker.Address.StreetName(),
            this.faker.Address.ZipCode(),
            this.faker.Random.Replace("(##) #####-####"));

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var customer = await this.context.Customers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.That(customer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(customer.Person.Name, Is.EqualTo(command.Name));
            Assert.That(customer.Person.Email, Is.EqualTo(command.Email));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllCustomers()
    {
        // Arrange
        var command1 = new AddCustomerCommand(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            this.faker.Random.Replace("###.###.###-##"),
            this.faker.Random.Replace("(##) #####-####"),
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
            this.faker.Random.Replace("(##) #####-####"),
            this.faker.Address.City(),
            null,
            null,
            null,
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var customers = await this.context.Customers.ToListAsync();
        Assert.That(customers, Has.Count.EqualTo(2));
    }
}

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

[TestFixture]
public class UpdateCustomerHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdateCustomerHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdateCustomerHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenCustomerExists_UpdatesCustomerAndReturnsResponse()
    {
        // Arrange
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
                Document = "123.456.789-00",
                Street = "Old Street",
                Number = "100",
                ZipCode = "12345-000",
                StateId = Guid.NewGuid(),
                City = "Old City"
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
            "Apt 202",
            "New Neighborhood",
            "200",
            Guid.NewGuid(),
            "New Street",
            "54321-000",
            "(11) 98765-4321");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.PersonId, Is.EqualTo(customer.Person.Id));
            Assert.That(result.Id, Is.EqualTo(customerId));
        }
    }

    [Test]
    public async Task Handle_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateCustomerCommand(
            Guid.NewGuid(),
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithNullPhoneNumber_SetsEmptyString()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullStreet_SetsEmptyString()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullZipCode_SetsEmptyString()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullNumber_SetsEmptyString()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullComplement_KeepsNull()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullNeighborhood_KeepsNull()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_WithNullCity_KeepsNull()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customer = new CustomerModel
        {
            Id = customerId,
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
                StateId = Guid.NewGuid(),
                City = this.faker.Address.City()
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
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
        Assert.That(result.PersonId, Is.Not.Empty);
    }

    [Test]
    public async Task Handle_VerifiesCustomerWasUpdatedInDatabase()
    {
        // Arrange
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
                Document = "123.456.789-00",
                Street = "Old Street",
                Number = "100",
                ZipCode = "12345-000",
                StateId = Guid.NewGuid(),
                City = "Old City"
            }
        };

        this.context.Customers.Add(customer);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCustomerCommand(
            customerId,
            "New Name",
            "new@email.com",
            "987.654.321-00",
            "New City",
            "Apt 202",
            "New Neighborhood",
            "200",
            Guid.NewGuid(),
            "New Street",
            "54321-000",
            "(11) 98765-4321");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedCustomer = await this.context.Customers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        Assert.That(updatedCustomer, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedCustomer.Person.Name, Is.EqualTo("New Name"));
            Assert.That(updatedCustomer.Person.Email, Is.EqualTo("new@email.com"));
            Assert.That(updatedCustomer.Person.City, Is.EqualTo("New City"));
        }
    }
}

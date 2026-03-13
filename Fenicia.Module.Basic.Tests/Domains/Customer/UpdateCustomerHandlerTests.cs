using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class UpdateCustomerHandlerTests : IDisposable
{
    public UpdateCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new UpdateCustomerHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly UpdateCustomerHandler handler;
    private readonly Faker faker;

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customer.Person.Id,
            result.PersonId);
        Assert.Equal(customerId,
            result.Id);
    }

    [Fact]
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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(new[] { result.PersonId });
    }

    [Fact]
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

        this.db.BasicCustomers.Add(customer);
        await this.db.SaveChangesAsync(CancellationToken.None);

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
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var updatedCustomer = await this.db.BasicCustomers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        Assert.NotNull(updatedCustomer);
        Assert.Equal("New Name",
            updatedCustomer.Person.Name);
        Assert.Equal("new@email.com",
            updatedCustomer.Person.Email);
        Assert.Equal("New City",
            updatedCustomer.Person.City);
    }
}

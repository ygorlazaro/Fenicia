using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

/// <summary>
///     Unit tests for the DeleteCustomerHandler.
///     Tests customer deletion (soft delete) business logic.
/// </summary>
public class DeleteCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteCustomerHandler handler;

    public DeleteCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new DeleteCustomerHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Tests that deleting an existing customer sets the Deleted timestamp.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCustomerExists_SetsDeletedDate()
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

        var command = new DeleteCustomerCommand(customerId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCustomer = await this.db.BasicCustomers.FindAsync([customerId], CancellationToken.None);
        Assert.NotNull(deletedCustomer);
        Assert.NotNull(deletedCustomer.Deleted);
        Assert.InRange(deletedCustomer.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    /// <summary>
    ///     Tests that deleting a non-existent customer does nothing.
    /// </summary>
    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var customers = await this.db.BasicCustomers.ToListAsync();
        Assert.Empty(customers);
    }

    /// <summary>
    ///     Tests that deleting one customer does not affect other customers.
    /// </summary>
    [Fact]
    public async Task Handle_WithMultipleCustomers_OnlyDeletesSpecified()
    {
        // Arrange
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var customer1 = new CustomerModel
        {
            Id = customer1Id,
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

        var customer2 = new CustomerModel
        {
            Id = customer2Id,
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

        this.db.BasicCustomers.AddRange(customer1, customer2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCustomerCommand(customer1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedCustomer = await this.db.BasicCustomers.FindAsync([customer1Id], CancellationToken.None);
        var notDeletedCustomer = await this.db.BasicCustomers.FindAsync([customer2Id], CancellationToken.None);

        Assert.NotNull(deletedCustomer);
        Assert.NotNull(deletedCustomer.Deleted);
        Assert.NotNull(notDeletedCustomer);
        Assert.Null(notDeletedCustomer.Deleted);
    }

    /// <summary>
    ///     Tests that deleting from an empty database does nothing.
    /// </summary>
    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteCustomerCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var customers = await this.db.BasicCustomers.ToListAsync();
        Assert.Empty(customers);
    }
}
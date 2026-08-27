using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class DeleteCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteCustomerHandler handler;

    public DeleteCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteCustomerHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenCustomerExists_SetsDeletedDate()
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
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCustomerCommand(customerId);
        var beforeDelete = DateTime.Now;

        await handler.Handle(command, CancellationToken.None);

        var deletedCustomer = await db.BasicCustomers.FindAsync([customerId], CancellationToken.None);
        Assert.NotNull(deletedCustomer);
        Assert.NotNull(deletedCustomer.Deleted);
        Assert.InRange(deletedCustomer.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenCustomerDoesNotExist_DoesNothing()
    {

        var command = new DeleteCustomerCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var customers = await db.BasicCustomers.ToListAsync();
        Assert.Empty(customers);
    }

    [Fact]
    public async Task Handle_WithMultipleCustomers_OnlyDeletesSpecified()
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
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        var customer2 = new CustomerModel
        {
            Id = customer2Id,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##")
            }
        };

        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteCustomerCommand(customer1Id);

        await handler.Handle(command, CancellationToken.None);

        var deletedCustomer = await db.BasicCustomers.FindAsync([customer1Id], CancellationToken.None);
        var notDeletedCustomer = await db.BasicCustomers.FindAsync([customer2Id], CancellationToken.None);

        Assert.NotNull(deletedCustomer);
        Assert.NotNull(deletedCustomer.Deleted);
        Assert.NotNull(notDeletedCustomer);
        Assert.Null(notDeletedCustomer.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {

        var command = new DeleteCustomerCommand(Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var customers = await db.BasicCustomers.ToListAsync();
        Assert.Empty(customers);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class DeleteCustomerServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CustomerService service;

    public DeleteCustomerServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new CustomerService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerExists_SetsDeletedDate()
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
            Email = faker.Internet.Email(),
            Document = faker.Random.Replace("###.###.###-##"),
            PhoneNumber = faker.Random.Replace("(##) #####-####")
        };

        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Person = person,
            PersonId = person.Id
        };

        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteCustomerCommand(customer.Id), CancellationToken.None);

        var updated = await db.BasicCustomers.FindAsync(customer.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenCustomerDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeleteCustomerCommand(Guid.NewGuid()), CancellationToken.None);

        var count = await db.BasicCustomers.CountAsync();
        Assert.Equal(0, count);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Commands;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class UpdateCustomerServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CustomerService service;

    public UpdateCustomerServiceTests()
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
    public async Task UpdateAsync_WhenCustomerExists_ReturnsUpdateResponse()
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

        var newName = faker.Person.FullName;
        var command = new UpdateCustomerCommand(customer.Id, newName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customer.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        var command = new UpdateCustomerCommand(Guid.NewGuid(), faker.Person.FullName, faker.Internet.Email(), faker.Random.Replace("###.###.###-##"), faker.Random.Replace("(##) #####-####"), null);

        var result = await service.UpdateAsync(command, CancellationToken.None);

        Assert.Null(result);
    }
}

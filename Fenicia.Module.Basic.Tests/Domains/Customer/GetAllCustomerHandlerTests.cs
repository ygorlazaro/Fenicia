using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.Handlers;
using Fenicia.Module.Basic.Domains.Customer.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class GetAllCustomerHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllCustomerHandler handler;

    public GetAllCustomerHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllCustomerHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllCustomerQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task Handle_WithCustomers_ReturnsAllCustomers()
    {

        var customer1 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        var customer2 = new CustomerModel
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Phone.PhoneNumber()
            }
        };

        db.BasicCustomers.AddRange(customer1, customer2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        var result = await handler.Handle(query, CancellationToken.None);

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

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicCustomers.Add(customer);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        for (var i = 0; i < 5; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicCustomers.Add(customer);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(5, result.Total);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        for (var i = 0; i < 25; i++)
        {
            var customer = new CustomerModel
            {
                Id = Guid.NewGuid(),
                PersonId = Guid.NewGuid(),
                Person = new PersonModel
                {
                    Id = Guid.NewGuid(),
                    Name = $"{faker.Person.FullName} {i}",
                    Email = faker.Internet.Email(),
                    Document = faker.Random.Replace("###.###.###-##"),
                    PhoneNumber = faker.Phone.PhoneNumber()
                }
            };
            db.BasicCustomers.Add(customer);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllCustomerQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Data.Count);
        Assert.Equal(25, result.Total);
    }
}

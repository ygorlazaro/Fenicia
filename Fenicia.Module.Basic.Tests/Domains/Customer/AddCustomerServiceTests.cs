using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs.Commands;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class AddCustomerServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CustomerService service;

    public AddCustomerServiceTests()
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
    public async Task AddAsync_WithValidCommand_ReturnsAddCustomerResponse()
    {
        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var result = await service.AddAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(Guid.Empty, result.PersonId);
    }

    [Fact]
    public async Task AddAsync_VerifiesCustomerSavedToDatabase()
    {
        var command = new AddCustomerCommand(
            faker.Person.FullName,
            faker.Internet.Email(),
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
            null);

        var result = await service.AddAsync(command, CancellationToken.None);

        var customer = await db.BasicCustomers.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == result.Id);
        Assert.NotNull(customer);
        Assert.Equal(command.Name, customer.Person.Name);
        Assert.Equal(command.Email, customer.Person.Email);
    }
}

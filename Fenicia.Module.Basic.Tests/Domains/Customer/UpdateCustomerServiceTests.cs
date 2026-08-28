using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Equal(command.Id, result.Id);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
            customer.Id,
        db.BasicCustomers.Add(customer);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
    [Fact]
            faker.Internet.Email(),
        faker = new Faker();
            faker.Person.FullName,
            faker.Random.Replace("###.###.###-##"),
            faker.Random.Replace("(##) #####-####"),
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Customer;
            null);
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
    private readonly CustomerService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    public async Task UpdateAsync_WhenCustomerExists_ReturnsUpdatedCustomer()
public class UpdateCustomerServiceTests : IDisposable
    public UpdateCustomerServiceTests()
    public void Dispose()
        service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
        var command = new UpdateCustomerCommand(
        var companyContext = new TestCompanyContext();
        var customer = new CustomerModel
        var customerRepository = new CustomerRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var person = new PersonModel
        var personRepository = new PersonRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

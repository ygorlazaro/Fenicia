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
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Total);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
        db.BasicCustomers.Add(customer);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            Document = faker.Random.Replace("###.###.###-##"),
            Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
            Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Customer;
            PersonId = person.Id,
            Person = person,
            PhoneNumber = faker.Random.Replace("(##) #####-####"),
    private readonly CustomerService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    public async Task GetAllAsync_WhenCustomersExist_ReturnsPaginationWithCustomers()
    public async Task GetAllAsync_WhenNoCustomers_ReturnsEmptyPagination()
public class GetAllCustomerServiceTests : IDisposable
    public GetAllCustomerServiceTests()
    public void Dispose()
        service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
        var companyContext = new TestCompanyContext();
        var customer = new CustomerModel
        var customerRepository = new CustomerRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var person = new PersonModel
        var personRepository = new PersonRepository(db);
        var result = await service.GetAllAsync(new GetAllCustomerQuery(1, 10), CancellationToken.None);

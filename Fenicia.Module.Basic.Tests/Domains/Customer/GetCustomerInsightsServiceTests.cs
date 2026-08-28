using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.AtRiskCustomers);
        Assert.NotNull(result.RecentOrders);
        Assert.NotNull(result.Summary);
        Assert.NotNull(result.TopCustomers);
        companyId = companyContext.CompanyId;
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Customer;
    private readonly CustomerService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    public async Task GetInsightsAsync_ReturnsCustomerInsights()
public class GetCustomerInsightsServiceTests : IDisposable
    public GetCustomerInsightsServiceTests()
    public void Dispose()
        service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        var addressRepository = new AddressRepository(db);
        var companyContext = new TestCompanyContext();
        var customerRepository = new CustomerRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var personAddressRepository = new PersonAddressRepository(db);
        var personRepository = new PersonRepository(db);
        var result = await service.GetInsightsAsync(new GetCustomerInsightsQuery(90, 10, 60), CancellationToken.None);

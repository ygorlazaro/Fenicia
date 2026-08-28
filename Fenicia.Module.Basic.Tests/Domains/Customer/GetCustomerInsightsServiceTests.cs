using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class GetCustomerInsightsServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CustomerService service;
    private readonly Guid companyId;

    public GetCustomerInsightsServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var customerRepository = new CustomerRepository(db);
        var personRepository = new PersonRepository(db);
        var addressRepository = new AddressRepository(db);
        var personAddressRepository = new PersonAddressRepository(db);
        var dashboardRepository = new DashboardRepository(db);
        service = new CustomerService(customerRepository, personRepository, addressRepository, personAddressRepository, dashboardRepository);
        faker = new Faker();
        companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetInsightsAsync_ReturnsCustomerInsights()
    {
        var result = await service.GetInsightsAsync(new GetCustomerInsightsQuery(90, 10, 60), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Summary);
        Assert.NotNull(result.TopCustomers);
        Assert.NotNull(result.RecentOrders);
        Assert.NotNull(result.AtRiskCustomers);
    }
}

using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Customer;

public class GetCustomerInsightsServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly CustomerService service;

    public GetCustomerInsightsServiceTests()
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
    public async Task GetInsightsAsync_ReturnsInsightsResponse()
    {
        var result = await service.GetInsightsAsync(new GetCustomerInsightsQuery(90, 10, 60), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Summary);
        Assert.NotNull(result.TopCustomers);
        Assert.NotNull(result.RecentOrders);
        Assert.NotNull(result.AtRiskCustomers);
    }
}

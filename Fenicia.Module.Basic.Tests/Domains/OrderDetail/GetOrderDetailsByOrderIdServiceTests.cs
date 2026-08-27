using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class GetOrderDetailsByOrderIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderDetailService service;

    public GetOrderDetailsByOrderIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new OrderDetailService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByOrderIdAsync_WhenOrderHasDetails_ReturnsDetails()
    {
        var orderId = Guid.NewGuid();
        var detail = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = Guid.NewGuid(),
            Price = 10,
            Quantity = 1,
            DiscountAmount = 0,
            Subtotal = 10
        };
        db.BasicOrderDetails.Add(detail);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(orderId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByOrderIdAsync_WhenOrderHasNoDetails_ReturnsEmptyList()
    {
        var result = await service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Microsoft.EntityFrameworkCore;

        {
        };
    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
        db.BasicOrderDetails.Add(detail);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            DiscountAmount = 0,
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Id = Guid.NewGuid(),
namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;
            OrderId = orderId,
            Price = 10,
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderDetailService service;
            ProductId = Guid.NewGuid(),
    public async Task GetByOrderIdAsync_WhenOrderHasDetails_ReturnsDetails()
    public async Task GetByOrderIdAsync_WhenOrderHasNoDetails_ReturnsEmptyList()
public class GetOrderDetailsByOrderIdServiceTests : IDisposable
    public GetOrderDetailsByOrderIdServiceTests()
    public void Dispose()
            Quantity = 1,
        service = new OrderDetailService(orderDetailRepository);
            Subtotal = 10
        var companyContext = new TestCompanyContext();
        var detail = new OrderDetailModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new OrderDetailRepository(db);
        var orderId = Guid.NewGuid();
        var result = await service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(Guid.NewGuid()), CancellationToken.None);
        var result = await service.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(orderId), CancellationToken.None);

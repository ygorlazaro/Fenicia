using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs.Commands;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class DeleteOrderServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;

    public DeleteOrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new OrderService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SetsDeletedDate()
    {
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            TotalAmount = 100,
            DiscountAmount = 0,
            TotalQuantity = 1,
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.Cash
        };
        db.BasicOrders.Add(order);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteOrderCommand(order.Id), CancellationToken.None);

        var updated = await db.BasicOrders.FindAsync(order.Id);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderDoesNotExist_DoesNothing()
    {
        await service.DeleteAsync(new DeleteOrderCommand(Guid.NewGuid()), CancellationToken.None);

        var count = await db.BasicOrders.CountAsync();
        Assert.Equal(0, count);
    }
}

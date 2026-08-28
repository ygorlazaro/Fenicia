using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Inventory;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class DeleteOrderServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;
    private readonly Guid companyId;

    public DeleteOrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var orderRepository = new OrderRepository(db);
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var stockMovementRepository = new StockMovementRepository(db);
        var productRepository = new ProductRepository(db);
        service = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        faker = new Faker();
        companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SetsDeletedDate()
    {
        var customer = new CustomerModel
        {
            Id = Guid.NewGuid(),
            Person = new PersonModel
            {
                Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
                Email = faker.Internet.Email(),
                Document = faker.Random.Replace("###.###.###-##"),
                PhoneNumber = faker.Random.Replace("(##) #####-####")
            },
            PersonId = Guid.NewGuid(),
            CompanyId = companyId
        };
        db.BasicCustomers.Add(customer);
        await db.SaveChangesAsync(CancellationToken.None);

        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            OrderNumber = "ORD-001",
            UserId = Guid.NewGuid(),
            CustomerId = customer.Id,
            TotalAmount = 100,
            DiscountAmount = 0,
            TotalQuantity = 1,
            SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            PaymentMethod = PaymentMethod.Cash,
            Customer = customer,
            CompanyId = companyId
        };
        db.BasicOrders.Add(order);
        await db.SaveChangesAsync(CancellationToken.None);

        await service.DeleteAsync(new DeleteOrderCommand(order.Id), CancellationToken.None);

        var deletedOrder = await db.BasicOrders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == order.Id);
        Assert.NotNull(deletedOrder);
        Assert.NotNull(deletedOrder.Deleted);
    }
}

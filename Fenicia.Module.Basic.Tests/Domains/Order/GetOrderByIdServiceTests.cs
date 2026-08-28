using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Microsoft.EntityFrameworkCore;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;

            {
            },
        {
        };
    {
    }
{
}
        Assert.Equal(order.Id, result.Id);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
        companyId = companyContext.CompanyId;
            CompanyId = companyId
            Customer = customer,
            CustomerId = customer.Id,
        db.BasicCustomers.Add(customer);
        db.BasicOrders.Add(order);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
            DiscountAmount = 0,
                Document = faker.Random.Replace("###.###.###-##"),
                Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
                Id = Guid.NewGuid(),
            Id = Guid.NewGuid(),
                Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Order;
            OrderNumber = "ORD-001",
            PaymentMethod = PaymentMethod.Cash,
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
                PhoneNumber = faker.Random.Replace("(##) #####-####")
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    private readonly OrderService service;
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
public class GetOrderByIdServiceTests : IDisposable
    public GetOrderByIdServiceTests()
    public void Dispose()
            SaleDate = DateTime.UtcNow,
        service = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
            Status = OrderStatus.Pending,
            TotalAmount = 100,
            TotalQuantity = 1,
            UserId = Guid.NewGuid(),
        var companyContext = new TestCompanyContext();
        var customer = new CustomerModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var order = new OrderModel
        var orderRepository = new OrderRepository(db);
        var productRepository = new ProductRepository(db);
        var result = await service.GetByIdAsync(new GetOrderByIdQuery(order.Id), CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);

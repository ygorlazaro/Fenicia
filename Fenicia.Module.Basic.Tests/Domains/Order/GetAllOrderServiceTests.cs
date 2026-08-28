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

public class GetAllOrderServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;
    private readonly Guid companyId;

    public GetAllOrderServiceTests()
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
    public async Task GetAllAsync_WhenNoOrders_ReturnsEmptyPagination()
    {
        var result = await service.GetAllAsync(new GetAllOrderQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetAllAsync_WhenOrdersExist_ReturnsPaginationWithOrders()
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

        var result = await service.GetAllAsync(new GetAllOrderQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
    }
}

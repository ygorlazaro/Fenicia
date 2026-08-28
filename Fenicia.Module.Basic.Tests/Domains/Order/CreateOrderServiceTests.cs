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

public class CreateOrderServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;
    private readonly Guid companyId;

    public CreateOrderServiceTests()
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
    public async Task CreateAsync_WithValidCommand_ReturnsCreateOrderResponse()
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

        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicProductCategories.Add(category);
        await db.SaveChangesAsync(CancellationToken.None);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = faker.Commerce.ProductName(),
            SalesPrice = faker.Random.Decimal(10, 100),
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true,
            CompanyId = companyId
        };
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            customer.Id,
            DateTime.UtcNow,
            OrderStatus.Pending,
            new List<OrderDetailCommand>
            {
                new OrderDetailCommand(product.Id, product.SalesPrice, 1)
            },
            PaymentMethod.Cash);

        var result = await service.CreateAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}

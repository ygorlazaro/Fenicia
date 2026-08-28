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
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result);
        await db.SaveChangesAsync(CancellationToken.None);
            CategoryId = category.Id,
        companyId = companyContext.CompanyId;
            CompanyId = companyId
            customer.Id,
            DateTime.UtcNow,
        db.BasicCustomers.Add(customer);
        db.BasicProductCategories.Add(category);
        db.BasicProducts.Add(product);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
                Document = faker.Random.Replace("###.###.###-##"),
                Email = faker.Internet.Email(),
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
            Guid.NewGuid(),
                Id = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            IsActive = true,
            Name = faker.Commerce.ProductName(),
                Name = faker.Person.FullName,
namespace Fenicia.Module.Basic.Tests.Domains.Order;
            new List<OrderDetailCommand>
                new OrderDetailCommand(product.Id, product.SalesPrice, 1)
            OrderStatus.Pending,
            PaymentMethod.Cash);
            PersonId = Guid.NewGuid(),
            Person = new PersonModel
                PhoneNumber = faker.Random.Replace("(##) #####-####")
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    private readonly OrderService service;
    public async Task CreateAsync_WithValidCommand_ReturnsCreateOrderResponse()
public class CreateOrderServiceTests : IDisposable
    public CreateOrderServiceTests()
    public void Dispose()
            Quantity = 100,
            SalesPrice = faker.Random.Decimal(10, 100),
        service = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var command = new CreateOrderCommand(
        var companyContext = new TestCompanyContext();
        var customer = new CustomerModel
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var orderRepository = new OrderRepository(db);
        var product = new ProductModel
        var productRepository = new ProductRepository(db);
        var result = await service.CreateAsync(command, companyId, CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);

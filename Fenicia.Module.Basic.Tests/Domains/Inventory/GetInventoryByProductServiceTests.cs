using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Inventory;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly InventoryService service;
    public async Task GetByProductAsync_ReturnsInventoryResponse()
public class GetInventoryByProductServiceTests : IDisposable
    public GetInventoryByProductServiceTests()
    public void Dispose()
        service = new InventoryService(productRepository, stockMovementRepository, orderDetailRepository, customerRepository, employeeRepository, supplierRepository);
        var companyContext = new TestCompanyContext();
        var customerRepository = new CustomerRepository(db);
        var employeeRepository = new EmployeeRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new OrderDetailRepository(db);
        var productId = Guid.NewGuid();
        var productRepository = new ProductRepository(db);
        var result = await service.GetByProductAsync(new GetInventoryByProductQuery(productId, 1, 10), CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);
        var supplierRepository = new SupplierRepository(db);

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryByProductServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly InventoryService service;

    public GetInventoryByProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var productRepository = new ProductRepository(db);
        var stockMovementRepository = new StockMovementRepository(db);
        var orderDetailRepository = new OrderDetailRepository(db);
        var customerRepository = new CustomerRepository(db);
        var employeeRepository = new EmployeeRepository(db);
        var supplierRepository = new SupplierRepository(db);
        service = new InventoryService(productRepository, stockMovementRepository, orderDetailRepository, customerRepository, employeeRepository, supplierRepository);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByProductAsync_ReturnsInventoryResponse()
    {
        var productId = Guid.NewGuid();
        var result = await service.GetByProductAsync(new GetInventoryByProductQuery(productId, 1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
    }
}

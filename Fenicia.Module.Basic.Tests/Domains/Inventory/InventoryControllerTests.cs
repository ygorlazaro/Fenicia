using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Moq;
using Bogus;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class InventoryControllerTests : IDisposable
{
    private readonly InventoryController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;

    public InventoryControllerTests()
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
        var service = new InventoryService(productRepository, stockMovementRepository, orderDetailRepository, customerRepository, employeeRepository, supplierRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new InventoryController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetInventoryAsync_ReturnsOk()
    {
        var wide = new WideEventContext();
        
        var result = await controller.GetInventoryAsync(wide, 1, 10, CancellationToken.None);
        
        Assert.IsType<OkObjectResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

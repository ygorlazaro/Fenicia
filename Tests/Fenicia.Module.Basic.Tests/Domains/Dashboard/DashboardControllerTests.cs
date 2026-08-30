using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Address;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.Person;
using Fenicia.Module.Basic.Domains.PersonAddress;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Tests.Domains.Dashboard;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class DashboardControllerTests : IDisposable
{
    private readonly DashboardController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;

    public DashboardControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new TestCompanyContext());
        var orderDetailRepository = new OrderDetailRepository(_db);
        var dummyStockMovementService = new StockMovementService();
        var productService = new ProductService(new ProductRepository(_db), new ProductCategoryService(new ProductCategoryRepository(_db)), new OrderDetailService(orderDetailRepository), dummyStockMovementService);
        var employeeService = new EmployeeService(new EmployeeRepository(_db), new PersonService(new PersonRepository(_db)), new AddressService(new AddressRepository(_db)), new PersonAddressService(new PersonAddressRepository(_db)), new OrderService(new OrderRepository(_db), new OrderDetailService(orderDetailRepository), new StockMovementService(new StockMovementRepository(_db), productService)));
        var service = new DashboardService(new OrderService(new OrderRepository(_db), new OrderDetailService(orderDetailRepository), new StockMovementService(new StockMovementRepository(_db), productService)), productService, employeeService);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new DashboardController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_ReturnsOk()
    {
        var wide = new WideEventContext();
        var result = await _controller.GetFinancialDashboardAsync(wide, 90, CancellationToken.None);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

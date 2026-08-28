using System.Security.Claims;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;

    {
    }
{
}
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.NotNull(authorizeAttribute);
        companyId = companyContext.CompanyId;
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
        controller = new OrderController(orderService, orderDetailService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
        mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
namespace Fenicia.Module.Basic.Tests.Domains.Order;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    private readonly Guid testUserId;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly OrderController controller;
    private void SetupUserClaims(Guid userId)
    public async Task GetAsync_WhenNoOrders_ReturnsOkWithEmptyPagination()
public class OrderControllerTests : IDisposable
    public OrderControllerTests()
    public void Dispose()
    public void OrderController_HasAuthorizeAttribute()
        SetupUserClaims(testUserId);
        testUserId = Guid.NewGuid();
        var authorizeAttribute = typeof(OrderController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        var orderRepository = new OrderRepository(db);
        var orderService = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        var productRepository = new ProductRepository(db);
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);
        var wide = new WideEventContext();

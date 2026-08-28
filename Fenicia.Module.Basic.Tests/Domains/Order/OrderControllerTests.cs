using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Fenicia.Common;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Fenicia.Module.Basic.Domains.Inventory;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Auth;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderControllerTests : IDisposable
{
    private readonly OrderController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Guid companyId;

    public OrderControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var orderRepository = new OrderRepository(db);
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var stockMovementRepository = new StockMovementRepository(db);
        var productRepository = new ProductRepository(db);
        var orderService = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        var orderDetailService = new OrderDetailService(orderDetailRepository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new OrderController(orderService, orderDetailService) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        companyId = companyContext.CompanyId;
        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenNoOrders_ReturnsOkWithEmptyPagination()
    {
        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        var authorizeAttribute = typeof(OrderController).GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();
        Assert.NotNull(authorizeAttribute);
    }
}

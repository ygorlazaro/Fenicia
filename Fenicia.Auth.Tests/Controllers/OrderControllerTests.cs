using System.Security.Claims;
using Fenicia.Auth.Domains.Order;
using Fenicia.Common.Api;
using Fenicia.Common.API;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Controllers;

public class OrderControllerTests
{
    private Mock<IOrderService> orderServiceMock;
    private OrderController sut;

    [SetUp]
    public void Setup()
    {
        orderServiceMock = new Mock<IOrderService>();
        sut = new OrderController(orderServiceMock.Object);
    }

    [Test]
    public async Task CreateNewOrderAsync_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        var headers = new Headers { CompanyId = Guid.NewGuid() };
        var request = new OrderRequest();
        var response = new OrderResponse { Id = Guid.NewGuid() };

        orderServiceMock.Setup(x => x.CreateNewOrderAsync(userId, headers.CompanyId, request, CancellationToken.None)).ReturnsAsync(response);

        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        sut.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var wide = new WideEventContext();

        var result = await sut.CreateNewOrderAsync(request, headers, wide, CancellationToken.None);

        Assert.That(result.Result, Is.TypeOf<OkObjectResult>());
        var ok = result.Result as OkObjectResult;
        Assert.That(ok?.Value, Is.EqualTo(response));
    }
}

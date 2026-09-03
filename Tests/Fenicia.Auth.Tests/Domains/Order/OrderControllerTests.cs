using System.Security.Claims;
using Fenicia.Auth.Domains.Order;
using Fenicia.Auth.Domains.Order.DTOs;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Order;

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IOrderService> _mockService;
    private readonly Guid _testUserId;

    public OrderControllerTests()
    {
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _mockService = new Mock<IOrderService>();

        _controller = new OrderController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenUserDoesNotBelongToCompany_ReturnsForbid()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(_testUserId, Guid.NewGuid(), modules);
        var companyId = Guid.NewGuid();

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateNewOrderCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedAccessException());

        var result = await _controller.CreateNewOrderAsync(command, companyId, wide, cancellationToken);

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenModulesDoNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateNewOrderCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ItemNotExistsException("Modules not found"));

        var modules = new List<Guid> { Guid.NewGuid() };
        var command = new CreateNewOrderCommand(_testUserId, Guid.NewGuid(), modules);
        var companyId = Guid.NewGuid();

        var result = await _controller.CreateNewOrderAsync(command, companyId, wide, cancellationToken);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNewOrderAsync_WhenValidRequest_ReturnsCreatedWithOrder()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateNewOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateNewOrderResponse(orderId));

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(_testUserId, Guid.NewGuid(), modules);
        var companyId = Guid.NewGuid();

        var result = await _controller.CreateNewOrderAsync(command, companyId, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedResponse = Assert.IsType<CreateNewOrderResponse>(createdResult.Value);
        Assert.NotNull(returnedResponse);
        Assert.Equal(orderId, returnedResponse.OrderId);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task CreateNewOrderAsync_SetsWideEventContextUserId()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var moduleId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateNewOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateNewOrderResponse(orderId));

        var modules = new List<Guid> { moduleId };
        var command = new CreateNewOrderCommand(_testUserId, Guid.NewGuid(), modules);
        var companyId = Guid.NewGuid();

        await _controller.CreateNewOrderAsync(command, companyId, wide, cancellationToken);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public void OrderController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(OrderController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void OrderController_HasRouteAttribute()
    {
        var controllerType = typeof(OrderController);

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void OrderController_HasProducesAttribute()
    {
        var controllerType = typeof(OrderController);

        var producesAttribute =
            controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

        Assert.NotNull(producesAttribute);
        Assert.Equal("application/json", producesAttribute.ContentTypes.FirstOrDefault());
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
using System.Security.Claims;

using AwesomeAssertions;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderControllerTests : IDisposable
{
    private readonly OrderController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IOrderService> _mockService;

    public OrderControllerTests()
    {
        _mockService = new Mock<IOrderService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new OrderController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, OrderStatus.Pending, [], PaymentMethod.Cash);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetAsync_WhenOrdersExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOk()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(orderId, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.Is<GetOrderByIdQuery>(q => q.Id != It.IsAny<Guid>()), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetOrderByIdResponse?)null);

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllOrderQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllOrderResponse>>([], 0, 1, 10));

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetOrderByIdQuery q, CancellationToken _) => new GetOrderByIdResponse(q.Id, "ORD-123", Guid.NewGuid(), Guid.NewGuid(), "Customer", 100, 0, 1, DateTime.UtcNow, "Pending", PaymentMethod.Cash, null));

        _mockService.Setup(s => s.CreateAsync(It.IsAny<CreateOrderCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreateOrderCommand _, Guid _, CancellationToken _) => new CreateOrderResponse(Guid.NewGuid(), "ORD-123", Guid.NewGuid(), Guid.NewGuid(), 100, 0, 1, DateTime.UtcNow, OrderStatus.Pending, PaymentMethod.Cash, null, Guid.NewGuid()));

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteOrderCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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

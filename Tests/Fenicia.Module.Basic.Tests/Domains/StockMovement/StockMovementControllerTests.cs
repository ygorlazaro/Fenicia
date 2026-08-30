using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class StockMovementControllerTests : IDisposable
{
    private readonly StockMovementController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<StockMovementService> _mockService;

    public StockMovementControllerTests()
    {
        _mockService = new Mock<StockMovementService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new StockMovementController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
        SetupServiceMocks();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

    private void SetupServiceMocks()
    {
        _mockService.Setup(s => s.GetAsync(It.IsAny<GetStockMovementQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetStockMovementResponse>());

        _mockService.Setup(s => s.AddAsync(It.IsAny<AddStockMovementCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AddStockMovementCommand cmd, Guid companyId, CancellationToken ct) => new AddStockMovementResponse(cmd.Id, cmd.ProductId, cmd.Quantity, cmd.Date, cmd.Price, cmd.Type, null, null, null, null, cmd.Reason));

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateStockMovementCommand>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateStockMovementCommand cmd, Guid companyId, CancellationToken ct) => new UpdateStockMovementResponse(cmd.Id, cmd.ProductId, cmd.Quantity, cmd.Date, cmd.Price, cmd.Type, null, null, null, null, cmd.Reason));

        _mockService.Setup(s => s.GetDashboardAsync(It.IsAny<GetStockMovementDashboardQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockMovementDashboardResponse());
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddStockMovementCommand(Guid.NewGuid(), 5.0, DateTime.UtcNow, 100, StockMovementType.In, Guid.NewGuid(), null, null, null, null, "Test");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenMovementExists_ReturnsOk()
    {
        // Arrange
        var movementId = Guid.NewGuid();
        var command = new UpdateStockMovementCommand(movementId, Quantity: 10.0, Date: DateTime.UtcNow, Price: 100, Type: StockMovementType.Out, ProductId: Guid.NewGuid(), CustomerId: null, SupplierId: null, EmployeeId: null, OrderId: null, Reason: "Updated");
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PatchAsync(movementId, command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenMovementDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var command = new UpdateStockMovementCommand(Guid.NewGuid(), Quantity: 10.0, Date: DateTime.UtcNow, Price: 100, Type: StockMovementType.Out, ProductId: Guid.NewGuid(), CustomerId: null, SupplierId: null, EmployeeId: null, OrderId: null, Reason: "Updated");
        var wide = new WideEventContext();

        _mockService.Setup(s => s.UpdateAsync(It.Is<UpdateStockMovementCommand>(c => c.Id != It.IsAny<Guid>()), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateStockMovementResponse?)null);

        // Act
        var result = await _controller.PatchAsync(Guid.NewGuid(), command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAsync_WhenStockMovementsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, null, null, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetDashboardAsync(wide, 30, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
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

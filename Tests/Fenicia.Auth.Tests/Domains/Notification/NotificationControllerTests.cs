using System.Security.Claims;
using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationControllerTests
{
    private readonly NotificationController _controller;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<INotificationService> _mockService;
    private readonly Guid _testUserId;

    public NotificationControllerTests()
    {
        _testUserId = Guid.NewGuid();
        _mockService = new Mock<INotificationService>();
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new NotificationController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };

        SetupUserClaims(_testUserId);
    }

    [Fact]
    public async Task GetAsync_WhenNoNotificationsExist_ReturnsOkWithEmptyPagination()
    {
        var query = new PaginationQuery();
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllNotificationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Pagination<List<GetAllNotificationsResponse>>([], 0, query.Page, query.PerPage));

        var result = await _controller.GetAsync(wide, query.Page, query.PerPage, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        var query = new PaginationQuery();
        var wide = new WideEventContext();

        await _controller.GetAsync(wide, query.Page, query.PerPage, null, null, CancellationToken.None);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotificationExists_ReturnsOk()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetNotificationByIdResponse(id, "Test", "D", DateTime.UtcNow, null, false));

        var wide = new WideEventContext();

        var result = await _controller.GetByIdAsync(id, wide, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotificationDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetNotificationByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var command = new AddNotificationCommand("Test Title", "Test Desc", DateTime.UtcNow, "img.png");
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var headers = new Headers { CompanyId = Guid.NewGuid() };

        _mockService.Setup(s => s.AddAsync(command, headers.CompanyId, cancellationToken))
            .ReturnsAsync(new AddNotificationResponse(Guid.NewGuid()));

        var result = await _controller.PostAsync(command, headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public async Task PatchAsync_WhenNotificationExists_ReturnsOk()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateNotificationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdateNotificationResponse(id));

        var command = new UpdateNotificationCommand(id, "New Title", "New Desc", null, "img.png", true);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var headers = new Headers { CompanyId = Guid.NewGuid() };

        var result = await _controller.PatchAsync(command, id, headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task PatchAsync_WhenMarkingAsRead_ReturnsOk()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateNotificationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UpdateNotificationResponse(id));

        var command = new UpdateNotificationCommand(id, "T", "D", null, null, true);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var headers = new Headers { CompanyId = Guid.NewGuid() };

        var result = await _controller.PatchAsync(command, id, headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task PatchAsync_WhenNotificationDoesNotExist_ReturnsNotFound()
    {
        var command = new UpdateNotificationCommand(Guid.NewGuid(), "Title", "Desc", null, null, null);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var headers = new Headers { CompanyId = Guid.NewGuid() };

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateNotificationCommand>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateNotificationResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotificationExists_ReturnsNoContent()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(id, It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;
        var headers = new Headers { CompanyId = Guid.NewGuid() };

        var result = await _controller.DeleteAsync(id, headers, wide, cancellationToken);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void NotificationController_HasAuthorizeAttribute()
    {
        var controllerType = typeof(NotificationController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void NotificationController_HasRouteAttribute()
    {
        var controllerType = typeof(NotificationController);

        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void NotificationController_HasProducesAttribute()
    {
        var controllerType = typeof(NotificationController);

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
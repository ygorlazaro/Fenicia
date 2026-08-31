using System.Security.Claims;
using Bogus;

using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationControllerTests : IDisposable
{
    private readonly NotificationController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public NotificationControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _testUserId = Guid.NewGuid();
        var notificationRepository = new NotificationRepository(_db);
        var notificationService = new NotificationService(notificationRepository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new NotificationController(notificationService) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _faker = new Faker();

        SetupUserClaims(_testUserId);
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenNoNotificationsExist_ReturnsOkWithEmptyPagination()
    {
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetAsync(wide, query.Page, query.PerPage, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAsync_SetsWideEventContextUserId()
    {
        var query = new PaginationQuery(1, 10);
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        await _controller.GetAsync(wide, query.Page, query.PerPage, null, null, CancellationToken.None);

        Assert.Equal(_testUserId.ToString(), wide.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotificationExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "Test", Description = "D", Date = DateTime.UtcNow });
        await _db.SaveChangesAsync(CancellationToken.None);

        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

        var result = await _controller.GetByIdAsync(id, wide, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotificationDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var cancellationToken = CancellationToken.None;

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

        var result = await _controller.PostAsync(command, headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);
    }

    [Fact]
    public async Task PatchAsync_WhenNotificationExists_ReturnsOk()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "Old", Description = "D", Date = DateTime.UtcNow, Read = false });
        await _db.SaveChangesAsync(CancellationToken.None);

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
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Read = false });
        await _db.SaveChangesAsync(CancellationToken.None);

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

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), headers, wide, cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotificationExists_ReturnsNoContent()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await _db.SaveChangesAsync(CancellationToken.None);

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

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void NotificationController_HasProducesAttribute()
    {
        var controllerType = typeof(NotificationController);

        var producesAttribute = controllerType.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;

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

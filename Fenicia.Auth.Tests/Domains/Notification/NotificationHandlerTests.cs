using Bogus;

using Fenicia.Auth.Domains.Notification.Commands;
using Fenicia.Auth.Domains.Notification.Handlers;
using Fenicia.Auth.Domains.Notification.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;

    public NotificationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        db = new DefaultContext(options, new TestCompanyContext());
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddNotificationHandler_ShouldCreateNotification()
    {
        var handler = new AddNotificationHandler(db);
        var command = new AddNotificationCommand("Test", "Desc", DateTime.UtcNow, "img.png");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == result.Id);
        Assert.NotNull(notification);
        Assert.Equal("Test", notification.Title);
    }

    [Fact]
    public async Task GetAllNotificationsHandler_ShouldReturnPaginatedNotifications()
    {
        var handler = new GetAllNotificationsHandler(db);
        for (var i = 0; i < 5; i++)
        {
            db.AuthNotifications.Add(new NotificationModel { Title = $"N{i}", Description = "D", Date = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await handler.Handle(new GetAllNotificationsQuery(1, 10), CancellationToken.None);

        Assert.Equal(5, result.Total);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetNotificationByIdHandler_ShouldReturnNotification_WhenExists()
    {
        var handler = new GetNotificationByIdHandler(db);
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await handler.Handle(new GetNotificationByIdQuery(id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetNotificationByIdHandler_ShouldReturnNull_WhenNotExists()
    {
        var handler = new GetNotificationByIdHandler(db);
        var result = await handler.Handle(new GetNotificationByIdQuery(Guid.NewGuid()), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteNotificationHandler_ShouldCompleteWithoutError()
    {
        var handler = new DeleteNotificationHandler(db);
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await db.SaveChangesAsync(CancellationToken.None);

        await handler.Handle(new DeleteNotificationCommand(id), CancellationToken.None);
    }

    [Fact]
    public async Task DeleteNotificationHandler_ShouldNotThrow_WhenNotificationNotExists()
    {
        var handler = new DeleteNotificationHandler(db);
        await handler.Handle(new DeleteNotificationCommand(Guid.NewGuid()), CancellationToken.None);
    }

    [Fact]
    public async Task UpdateNotificationHandler_ShouldUpdateNotification_WhenExists()
    {
        var handler = new UpdateNotificationHandler(db);
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "Old", Description = "D", Date = DateTime.UtcNow, Read = false });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await handler.Handle(new UpdateNotificationCommand(id, "New", "D2", null, "img2.png", true), CancellationToken.None);

        Assert.NotNull(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.Equal("New", notification.Title);
        Assert.True(notification.Read);
    }

    [Fact]
    public async Task UpdateNotificationHandler_ShouldReturnNull_WhenNotExists()
    {
        var handler = new UpdateNotificationHandler(db);
        var result = await handler.Handle(new UpdateNotificationCommand(Guid.NewGuid(), "T", "D", null, null, null), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task MarkAsReadHandler_ShouldMarkNotificationAsRead_WhenExists()
    {
        var handler = new MarkAsReadHandler(db);
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Read = false });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await handler.Handle(new MarkAsReadCommand(id), CancellationToken.None);

        Assert.True(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.True(notification.Read);
    }

    [Fact]
    public async Task MarkAsReadHandler_ShouldReturnFalse_WhenNotExists()
    {
        var handler = new MarkAsReadHandler(db);
        var result = await handler.Handle(new MarkAsReadCommand(Guid.NewGuid()), CancellationToken.None);
        Assert.False(result);
    }
}

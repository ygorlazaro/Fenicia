using Bogus;

using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.DTOs.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly NotificationService service;

    public NotificationHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        db = new DefaultContext(options, new TestCompanyContext());
        faker = new Faker();
        service = new NotificationService(db);
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateNotification()
    {
        var command = new AddNotificationCommand("Test", "Desc", DateTime.UtcNow, "img.png");

        var result = await service.AddAsync(command, CancellationToken.None);

        Assert.NotNull(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == result.Id);
        Assert.NotNull(notification);
        Assert.Equal("Test", notification.Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedNotifications()
    {
        for (var i = 0; i < 5; i++)
        {
            db.AuthNotifications.Add(new NotificationModel { Title = $"N{i}", Description = "D", Date = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAllAsync(1, 10, CancellationToken.None);

        Assert.Equal(5, result.Total);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCompleteWithoutError()
    {
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.DeleteAsync(id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotificationNotExists()
    {
        var result = await service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "Old", Description = "D", Date = DateTime.UtcNow, Read = false });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.UpdateAsync(new UpdateNotificationCommand(id, "New", "D2", null, "img2.png", true), CancellationToken.None);

        Assert.NotNull(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.Equal("New", notification.Title);
        Assert.True(notification.Read);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await service.UpdateAsync(new UpdateNotificationCommand(Guid.NewGuid(), "T", "D", null, null, null), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldMarkNotificationAsRead_WhenExists()
    {
        var id = Guid.NewGuid();
        db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Read = false });
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.MarkAsReadAsync(id, CancellationToken.None);

        Assert.True(result);
        var notification = await db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.True(notification.Read);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await service.MarkAsReadAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }
}

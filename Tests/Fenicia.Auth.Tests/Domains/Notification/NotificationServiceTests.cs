using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new NotificationService(new NotificationRepository(_db));
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateNotification()
    {
        var command = new AddNotificationCommand("Test", "Desc", DateTime.UtcNow, "img.png");

        var result = await _service.AddAsync(command, _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        var notification = await _db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == result.Id);
        Assert.NotNull(notification);
        Assert.Equal("Test", notification.Title);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedNotifications()
    {
        for (var i = 0; i < 5; i++)
        {
            _db.AuthNotifications.Add(new NotificationModel { Title = $"N{i}", Description = "D", Date = DateTime.UtcNow });
        }

        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(1, 10, CancellationToken.None);

        Assert.Equal(5, result.Total);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCompleteWithoutError()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.DeleteAsync(id, _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotificationNotExists()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid(), _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "Old", Description = "D", Date = DateTime.UtcNow, Read = false });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.UpdateAsync(new UpdateNotificationCommand(id, "New", "D2", null, "img2.png", true), _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        var notification = await _db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.Equal("New", notification?.Title);
        Assert.True(notification?.Read);
    }

    [Fact]
    public async Task UpdateAsync_ShouldMarkAsRead_WhenIsReadIsTrue()
    {
        var id = Guid.NewGuid();
        _db.AuthNotifications.Add(new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Read = false });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.UpdateAsync(new UpdateNotificationCommand(id, "T", "D", null, null, true), _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        var notification = await _db.AuthNotifications.FirstOrDefaultAsync(n => n.Id == id);
        Assert.True(notification?.Read);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.UpdateAsync(new UpdateNotificationCommand(Guid.NewGuid(), "T", "D", null, null, null), _db.CurrentCompanyId ?? Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }
}

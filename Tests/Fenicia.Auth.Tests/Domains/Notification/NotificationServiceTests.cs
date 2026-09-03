using Fenicia.Auth.Domains.Notification;
using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Moq;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationServiceTests
{
    private readonly Mock<INotificationRepository> _mockRepository;
    private readonly NotificationService _service;

    public NotificationServiceTests()
    {
        _mockRepository = new Mock<INotificationRepository>();
        _service = new NotificationService(_mockRepository.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldCreateNotification()
    {
        var command = new AddNotificationCommand("Test", "Desc", DateTime.UtcNow, "img.png");
        var companyId = Guid.NewGuid();

        var createdNotification = new NotificationModel
        {
            Id = Guid.NewGuid(), Title = "Test", Description = "Desc", Date = DateTime.UtcNow, ImageUrl = "img.png",
            Read = false, CompanyId = companyId
        };
        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdNotification);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        _mockRepository.Verify(
            r => r.InsertAsync(It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPaginatedNotifications()
    {
        var notifications = new List<NotificationModel>();
        for (var i = 0; i < 5; i++)
        {
            notifications.Add(
                new NotificationModel
                    { Id = Guid.NewGuid(), Title = $"N{i}", Description = "D", Date = DateTime.UtcNow });
        }

        _mockRepository.Setup(r => r.Query()).Returns(notifications.AsAsyncQueryable());

        var result = await _service.GetAllAsync(new GetAllNotificationsQuery(), CancellationToken.None);

        Assert.Equal(5, result.Total);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        var notification = new NotificationModel { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _service.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationModel?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCompleteWithoutError()
    {
        var id = Guid.NewGuid();
        var notification = new NotificationModel
            { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Deleted = null };

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);
        _mockRepository.Setup(r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);

        var result = await _service.DeleteAsync(id, Guid.NewGuid(), CancellationToken.None);

        Assert.True(result);
        _mockRepository.Verify(
            r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotificationNotExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationModel?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        var existing = new NotificationModel
            { Id = id, Title = "Old", Description = "D", Date = DateTime.UtcNow, Read = false };

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.UpdateAsync(
            new UpdateNotificationCommand(id, "New", "D2", null, "img2.png", true),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.NotNull(result);
        _mockRepository.Verify(
            r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldMarkAsRead_WhenIsReadIsTrue()
    {
        var id = Guid.NewGuid();
        var existing = new NotificationModel
            { Id = id, Title = "T", Description = "D", Date = DateTime.UtcNow, Read = false };

        _mockRepository.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mockRepository.Setup(r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var result = await _service.UpdateAsync(
            new UpdateNotificationCommand(id, "T", "D", null, null, true),
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.NotNull(result);
        _mockRepository.Verify(
            r => r.UpdateAsync(id, It.IsAny<NotificationModel>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((NotificationModel?)null);

        var result = await _service.UpdateAsync(
            new UpdateNotificationCommand(Guid.NewGuid(), "T", "D", null, null, null),
            Guid.NewGuid(),
            CancellationToken.None);
        Assert.Null(result);
    }
}
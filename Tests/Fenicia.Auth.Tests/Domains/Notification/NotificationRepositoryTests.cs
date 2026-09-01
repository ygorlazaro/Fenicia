using Fenicia.Auth.Domains.Notification;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Notification;

public class NotificationRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly NotificationRepository _repository;

    public NotificationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new NotificationRepository(_db);
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllWithPaginationAsync_WhenNotificationsExist_ReturnsPaginatedResults()
    {
        for (var i = 0; i < 5; i++)
        {
            _db.AuthNotifications.Add(new NotificationModel
            {
                Title = $"Notification {i}",
                Description = "Desc",
                Date = DateTime.UtcNow.AddDays(-i)
            });
        }

        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllWithPaginationAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(5, result.Total);
        Assert.Equal(5, result.Data.Count);
    }

    [Fact]
    public async Task GetAllWithPaginationAsync_WhenNoNotificationsExist_ReturnsEmptyPagination()
    {
        var result = await _repository.GetAllWithPaginationAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.Total);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetAllWithPaginationAsync_ResultsAreOrderedByDateDescending()
    {
        var oldest = new NotificationModel { Title = "Oldest", Description = "D", Date = DateTime.UtcNow.AddDays(-3) };
        var newest = new NotificationModel { Title = "Newest", Description = "D", Date = DateTime.UtcNow.AddDays(0) };
        var middle = new NotificationModel { Title = "Middle", Description = "D", Date = DateTime.UtcNow.AddDays(-1) };

        _db.AuthNotifications.AddRange(oldest, newest, middle);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllWithPaginationAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Data.Count);
        Assert.Equal("Newest", result.Data[0].Title);
        Assert.Equal("Middle", result.Data[1].Title);
        Assert.Equal("Oldest", result.Data[2].Title);
    }

    [Fact]
    public async Task GetAllWithPaginationAsync_WhenPageExceedsTotal_ReturnsEmptyData()
    {
        _db.AuthNotifications.Add(new NotificationModel { Title = "N1", Description = "D", Date = DateTime.UtcNow });
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllWithPaginationAsync(10, 10, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Empty(result.Data);
    }
}

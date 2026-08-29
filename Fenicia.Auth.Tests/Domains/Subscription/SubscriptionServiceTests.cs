using Fenicia.Auth.Domains.Subscription;
using Fenicia.Auth.Domains.Subscription.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.Subscription;

public class SubscriptionServiceTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        _context = new DefaultContext(options, new TestCompanyContext());
        _context.Database.OpenConnection();
        _context.Database.EnsureCreated();

        _service = new SubscriptionService(new SubscriptionRepository(_context));
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var nonExistentUserId = Guid.NewGuid();

        var result = await _service.GetUserProfileAsync(nonExistentUserId, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserExists_ReturnsProfileWithEmptyCollections()
    {
        var userId = Guid.NewGuid();
        var user = new UserModel
        {
            Id = userId,
            Name = "Test User",
            Email = "test@example.com",
            Created = DateTime.UtcNow
        };

        _context.AuthUsers.Add(user);
        await _context.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetUserProfileAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("test@example.com", result.Email);
        Assert.Empty(result.Companies);
        Assert.Empty(result.Subscriptions);
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}

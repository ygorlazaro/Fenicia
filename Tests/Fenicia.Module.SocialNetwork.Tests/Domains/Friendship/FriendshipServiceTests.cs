using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Friendship;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Friendship;

public class FriendshipServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly FriendshipService _service;

    public FriendshipServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new FriendshipRepository(_db);
        _service = new FriendshipService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task FollowAsync_WhenNotFollowing_CreatesFriendship()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var result = await _service.FollowAsync(new FollowCommand(targetUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.TargetUserId.Should().Be(targetUserId);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task FollowAsync_WhenAlreadyFollowing_ReturnsExistingFriendship()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var friendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = targetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkFriendships.Add(friendship);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.FollowAsync(new FollowCommand(targetUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(friendship.Id);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task FollowAsync_WhenPreviouslyUnfollowed_ReactivatesFriendship()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var friendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = targetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = false
        };
        _db.SocialNetworkFriendships.Add(friendship);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.FollowAsync(new FollowCommand(targetUserId), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(friendship.Id);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UnfollowAsync_WhenFollowing_SetsInactive()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var friendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = targetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkFriendships.Add(friendship);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.UnfollowAsync(new UnfollowCommand(targetUserId), userId, CancellationToken.None);

        var updated = await _db.SocialNetworkFriendships.IgnoreQueryFilters().FirstOrDefaultAsync(f => f.Id == friendship.Id);
        updated.Should().NotBeNull();
        updated!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task UnfollowAsync_WhenNotFollowing_DoesNothing()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        await _service.UnfollowAsync(new UnfollowCommand(targetUserId), userId, CancellationToken.None);

        var count = await _db.SocialNetworkFriendships.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetFollowersAsync_WhenFollowersExist_ReturnsPagination()
    {
        var targetUserId = Guid.NewGuid();
        var follower = new FriendshipModel
        {
            UserId = Guid.NewGuid(),
            TargetUserId = targetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkFriendships.Add(follower);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetFollowersAsync(new GetFollowersQuery(1, 10), targetUserId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().UserId.Should().Be(follower.UserId);
    }

    [Fact]
    public async Task GetFollowingAsync_WhenFollowingExist_ReturnsPagination()
    {
        var userId = Guid.NewGuid();
        var following = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = Guid.NewGuid(),
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkFriendships.Add(following);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetFollowingAsync(new GetFollowingQuery(1, 10), userId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().TargetUserId.Should().Be(following.TargetUserId);
    }

    [Fact]
    public async Task IsFollowingAsync_WhenFollowing_ReturnsTrue()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();
        var friendship = new FriendshipModel
        {
            UserId = userId,
            TargetUserId = targetUserId,
            FollowDate = DateTime.UtcNow,
            IsActive = true
        };
        _db.SocialNetworkFriendships.Add(friendship);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.IsFollowingAsync(new IsFollowingQuery(targetUserId), userId, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsFollowingAsync_WhenNotFollowing_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        var result = await _service.IsFollowingAsync(new IsFollowingQuery(targetUserId), userId, CancellationToken.None);

        result.Should().BeFalse();
    }
}

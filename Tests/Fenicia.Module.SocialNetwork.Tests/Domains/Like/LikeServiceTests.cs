using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Fenicia.Module.SocialNetwork.Domains.Like.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Like;

public class LikeServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly LikeService _service;
    private readonly Guid _companyId;

    public LikeServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new LikeService(new LikeRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task LikeAsync_WhenNoExistingLike_CreatesLike()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var command = new LikeCommand(feedId);

        // Act
        var result = await _service.LikeAsync(command, _companyId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.FeedId.Should().Be(feedId);
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task LikeAsync_WhenLikeAlreadyExists_ReturnsExistingLike()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var existingLike = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(existingLike);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new LikeCommand(feedId);

        // Act
        var result = await _service.LikeAsync(command, _companyId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingLike.Id);
    }

    [Fact]
    public async Task UnlikeAsync_WhenLikeExists_RemovesLike()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.UnlikeAsync(new UnlikeCommand(feedId), userId, CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkLikes.IgnoreQueryFilters().CountAsync();
        count.Should().Be(1);
        var deletedLike = await _db.SocialNetworkLikes.IgnoreQueryFilters().FirstOrDefaultAsync(l => l.Id == like.Id);
        deletedLike!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task UnlikeAsync_WhenLikeDoesNotExist_DoesNothing()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();

        // Act
        await _service.UnlikeAsync(new UnlikeCommand(feedId), userId, CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkLikes.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetLikesByFeedAsync_WhenLikesExist_ReturnsLikes()
    {
        // Arrange
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetLikesByFeedAsync(new GetLikesByFeedQuery(1, 10, feedId, null, null), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(like.Id);
    }

    [Fact]
    public async Task IsLikedAsync_WhenLikeExists_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.IsLikedAsync(new IsLikedQuery(), userId, feedId, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsLikedAsync_WhenLikeDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();

        // Act
        var result = await _service.IsLikedAsync(new IsLikedQuery(), userId, feedId, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Like;

public class LikeRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly LikeRepository _repository;

    public LikeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new LikeRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByUserAndFeedAsync_WhenLikeExists_ReturnsLike()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByUserAndFeedAsync(userId, feedId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(like.Id);
    }

    [Fact]
    public async Task GetByUserAndFeedAsync_WhenLikeDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByUserAndFeedAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByFeedAsync_ReturnsLikesForFeed()
    {
        // Arrange
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = feedId,
            LikeDate = DateTime.UtcNow
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByFeedAsync(feedId, cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(like.Id);
    }
}

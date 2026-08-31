using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Feed;

public class FeedRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly FeedRepository _repository;

    public FeedRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new FeedRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsFeedsOrderedByDateDescending()
    {
        // Arrange
        var feed1 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-2), Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid() };
        var feed2 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-1), Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid() };
        var feed3 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid() };
        _db.SocialNetworkFeeds.AddRange(feed1, feed2, feed3);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.First().Id.Should().Be(feed3.Id);
        result.Last().Id.Should().Be(feed1.Id);
    }

    [Fact]
    public async Task GetByIdWithRelationsAsync_WhenFeedExists_ReturnsFeedWithRelations()
    {
        // Arrange
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = Guid.NewGuid(),
            Comments = [],
            Likes = [],
            Shares = []
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdWithRelationsAsync(feed.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(feed.Id);
    }

    [Fact]
    public async Task GetByIdWithRelationsAsync_WhenFeedDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdWithRelationsAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

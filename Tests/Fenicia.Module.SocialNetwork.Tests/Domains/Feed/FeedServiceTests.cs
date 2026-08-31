using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Feed;

public class FeedServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly FeedService _service;
    private readonly Guid _companyId;

    public FeedServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new FeedService(new FeedRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenFeedsExist_ReturnsPaginationWithFeeds()
    {
        // Arrange
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllFeedQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(feed.Id);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByDateDescending()
    {
        // Arrange
        var feed1 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-2), Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid(), CompanyId = _companyId };
        var feed2 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-1), Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid(), CompanyId = _companyId };
        var feed3 = new FeedModel { Id = Guid.NewGuid(), Date = DateTime.UtcNow, Text = _faker.Lorem.Sentence(), UserId = Guid.NewGuid(), CompanyId = _companyId };
        _db.SocialNetworkFeeds.AddRange(feed1, feed2, feed3);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllFeedQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.First().Id.Should().Be(feed3.Id);
        result.Last().Id.Should().Be(feed1.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFeedExists_ReturnsFeed()
    {
        // Arrange
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetFeedByIdQuery(feed.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(feed.Id);
        result.Text.Should().Be(feed.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFeedDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetFeedByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesFeed()
    {
        // Arrange
        var command = new AddFeedCommand(Guid.NewGuid(), DateTime.UtcNow, _faker.Lorem.Sentence(), Guid.NewGuid());

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Text.Should().Be(command.Text);
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenFeedExists_UpdatesFeed()
    {
        // Arrange
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateFeedCommand(feed.Id, DateTime.UtcNow, _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(feed.Id);
        result.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task UpdateAsync_WhenFeedDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateFeedCommand(Guid.NewGuid(), DateTime.UtcNow, _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenFeedExists_SoftDeletesFeed()
    {
        // Arrange
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteFeedCommand(feed.Id), CancellationToken.None);

        // Assert
        var deletedFeed = await _db.SocialNetworkFeeds.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == feed.Id);
        deletedFeed.Should().NotBeNull();
        deletedFeed!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenFeedDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteFeedCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkFeeds.CountAsync();
        count.Should().Be(0);
    }
}

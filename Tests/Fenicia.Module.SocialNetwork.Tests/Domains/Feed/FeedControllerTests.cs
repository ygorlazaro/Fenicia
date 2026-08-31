using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Feed;
using Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Feed;

public class FeedControllerTests : IDisposable
{
    private readonly FeedController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public FeedControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new FeedRepository(_db);
        var service = new FeedService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new FeedController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _testUserId = Guid.NewGuid();
        _companyId = companyContext.CompanyId;
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenFeedsExist_ReturnsOkWithFeeds()
    {
        // Arrange
        var wide = new WideEventContext();
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = _testUserId,
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var feeds = (List<GetAllFeedResponse>)okResult.Value!;
        feeds.Should().HaveCount(1);
        feeds.First().Id.Should().Be(feed.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNoFeedsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var feeds = (List<GetAllFeedResponse>)okResult.Value!;
        feeds.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenFeedExists_ReturnsOkWithFeed()
    {
        // Arrange
        var wide = new WideEventContext();
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = _testUserId,
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(feed.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedFeed = (GetFeedByIdResponse)okResult.Value!;
        returnedFeed.Id.Should().Be(feed.Id);
        returnedFeed.Text.Should().Be(feed.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WhenFeedDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddFeedCommand(Guid.NewGuid(), DateTime.UtcNow, _faker.Lorem.Sentence(), _testUserId);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedFeed = (AddFeedResponse)createdResult.Value!;
        returnedFeed.Id.Should().Be(command.Id);
        returnedFeed.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task PatchAsync_WhenFeedExists_ReturnsOkWithUpdatedFeed()
    {
        // Arrange
        var wide = new WideEventContext();
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = _testUserId,
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateFeedCommand(feed.Id, DateTime.UtcNow, _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, feed.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedFeed = (UpdateFeedResponse)okResult.Value!;
        returnedFeed.Id.Should().Be(feed.Id);
        returnedFeed.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task PatchAsync_WhenFeedDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateFeedCommand(Guid.NewGuid(), DateTime.UtcNow, _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenFeedExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var feed = new FeedModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Text = _faker.Lorem.Sentence(),
            UserId = _testUserId,
            CompanyId = _companyId
        };
        _db.SocialNetworkFeeds.Add(feed);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(feed.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenFeedDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

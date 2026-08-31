using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Like;
using Fenicia.Module.SocialNetwork.Domains.Like.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Like;

public class LikeControllerTests : IDisposable
{
    private readonly LikeController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public LikeControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new LikeRepository(_db);
        var service = new LikeService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new LikeController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var feedId = Guid.NewGuid();
        var command = new LikeCommand(feedId);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedLike = (AddLikeResponse)createdResult.Value!;
        returnedLike.FeedId.Should().Be(feedId);
    }

    [Fact]
    public async Task UnlikeAsync_WhenLikeExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.UnlikeAsync(feedId, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetLikesByFeedAsync_WhenLikesExist_ReturnsOkWithLikes()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();
        var like = new LikeModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = feedId,
            LikeDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkLikes.Add(like);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetLikesByFeedAsync(feedId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        var likes = (List<GetLikesResponse>)okResult.Value!;
        likes.Should().HaveCount(1);
        likes.First().Id.Should().Be(like.Id);
    }

    [Fact]
    public async Task IsLikedAsync_WhenLiked_ReturnsTrue()
    {
        // Arrange
        var wide = new WideEventContext();
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
        var result = await _controller.IsLikedAsync(userId, feedId, wide, CancellationToken.None);

        // Assert
        var okResult = (OkObjectResult)result.Result!;
        var isLiked = (bool)okResult.Value!;
        isLiked.Should().BeTrue();
    }

    [Fact]
    public async Task IsLikedAsync_WhenNotLiked_ReturnsFalse()
    {
        // Arrange
        var wide = new WideEventContext();
        var userId = Guid.NewGuid();
        var feedId = Guid.NewGuid();

        // Act
        var result = await _controller.IsLikedAsync(userId, feedId, wide, CancellationToken.None);

        // Assert
        var okResult = (OkObjectResult)result.Result!;
        var isLiked = (bool)okResult.Value!;
        isLiked.Should().BeFalse();
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

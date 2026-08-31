using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Comment;

public class CommentControllerTests : IDisposable
{
    private readonly CommentController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public CommentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new CommentRepository(_db);
        var service = new CommentService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new CommentController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task GetByFeedAsync_WhenCommentsExist_ReturnsOkWithComments()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = feedId,
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByFeedAsync(feedId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var comments = (List<GetAllCommentResponse>)okResult.Value!;
        comments.Should().HaveCount(1);
        comments.First().Id.Should().Be(comment.Id);
    }

    [Fact]
    public async Task GetByFeedAsync_WhenNoCommentsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var feedId = Guid.NewGuid();

        // Act
        var result = await _controller.GetByFeedAsync(feedId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var comments = (List<GetAllCommentResponse>)okResult.Value!;
        comments.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsOkWithComment()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(comment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedComment = (GetCommentByIdResponse)okResult.Value!;
        returnedComment.Id.Should().Be(comment.Id);
        returnedComment.Text.Should().Be(comment.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNotFound()
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
        var command = new AddCommentCommand(Guid.NewGuid(), _testUserId, Guid.NewGuid(), null, _faker.Lorem.Sentence());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedComment = (AddCommentResponse)createdResult.Value!;
        returnedComment.Id.Should().Be(command.Id);
        returnedComment.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task PatchAsync_WhenCommentExists_ReturnsOkWithUpdatedComment()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCommentCommand(comment.Id, _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, comment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedComment = (UpdateCommentResponse)okResult.Value!;
        returnedComment.Id.Should().Be(comment.Id);
        returnedComment.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task PatchAsync_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateCommentCommand(Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(comment.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetRepliesAsync_WhenRepliesExist_ReturnsOkWithReplies()
    {
        // Arrange
        var wide = new WideEventContext();
        var parentCommentId = Guid.NewGuid();
        var reply = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = _testUserId,
            FeedId = Guid.NewGuid(),
            ParentCommentId = parentCommentId,
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(reply);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetRepliesAsync(parentCommentId, wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var replies = (List<GetRepliesResponse>)okResult.Value!;
        replies.Should().HaveCount(1);
        replies.First().Id.Should().Be(reply.Id);
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

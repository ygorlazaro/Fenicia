using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Comment;

public class CommentServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CommentService _service;
    private readonly Guid _companyId;

    public CommentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new CommentService(new CommentRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllByFeedAsync_WhenCommentsExist_ReturnsPaginationWithComments()
    {
        // Arrange
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllByFeedAsync(new GetAllCommentByFeedQuery(1, 10, comment.FeedId), comment.FeedId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(comment.Id);
    }

    [Fact]
    public async Task GetAllByFeedAsync_OrdersByCommentDateAscending()
    {
        // Arrange
        var feedId = Guid.NewGuid();
        var comment1 = new CommentModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), FeedId = feedId, Text = _faker.Lorem.Sentence(), CommentDate = DateTime.UtcNow.AddDays(-2), CompanyId = _companyId };
        var comment2 = new CommentModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), FeedId = feedId, Text = _faker.Lorem.Sentence(), CommentDate = DateTime.UtcNow.AddDays(-1), CompanyId = _companyId };
        var comment3 = new CommentModel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), FeedId = feedId, Text = _faker.Lorem.Sentence(), CommentDate = DateTime.UtcNow, CompanyId = _companyId };
        _db.SocialNetworkComments.AddRange(comment1, comment2, comment3);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllByFeedAsync(new GetAllCommentByFeedQuery(1, 10, feedId), feedId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.First().Id.Should().Be(comment1.Id);
        result.Last().Id.Should().Be(comment3.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        // Arrange
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetCommentByIdQuery(comment.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(comment.Id);
        result.Text.Should().Be(comment.Text);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetCommentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new AddCommentCommand(Guid.NewGuid(), userId, Guid.NewGuid(), null, _faker.Lorem.Sentence());

        // Act
        var result = await _service.AddAsync(command, _companyId, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(_companyId);
        result.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentExists_UpdatesComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCommentCommand(comment.Id, _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, userId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(comment.Id);
        result.Text.Should().Be(command.Text);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateCommentCommand(Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenUserIsNotOwner_ReturnsNull()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = ownerId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateCommentCommand(comment.Id, _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, otherUserId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentExists_SoftDeletesComment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteCommentCommand(comment.Id), userId, CancellationToken.None);

        // Assert
        var deletedComment = await _db.SocialNetworkComments.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == comment.Id);
        deletedComment.Should().NotBeNull();
        deletedComment!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteCommentCommand(Guid.NewGuid()), Guid.NewGuid(), CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkComments.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserIsNotOwner_DoesNothing()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = ownerId,
            FeedId = Guid.NewGuid(),
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteCommentCommand(comment.Id), otherUserId, CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkComments.IgnoreQueryFilters().CountAsync();
        count.Should().Be(1);
    }

    [Fact]
    public async Task GetRepliesAsync_WhenRepliesExist_ReturnsReplies()
    {
        // Arrange
        var parentCommentId = Guid.NewGuid();
        var reply = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = Guid.NewGuid(),
            ParentCommentId = parentCommentId,
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow,
            CompanyId = _companyId
        };
        _db.SocialNetworkComments.Add(reply);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetRepliesAsync(new GetRepliesQuery(1, 10, parentCommentId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(reply.Id);
    }
}

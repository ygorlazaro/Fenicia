using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Comment;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Comment;

public class CommentRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly CommentRepository _repository;

    public CommentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new CommentRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByFeedAsync_ReturnsCommentsForFeed()
    {
        // Arrange
        var feedId = Guid.NewGuid();
        var comment = new CommentModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            FeedId = feedId,
            Text = _faker.Lorem.Sentence(),
            CommentDate = DateTime.UtcNow
        };
        _db.SocialNetworkComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByFeedAsync(feedId, ct: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(comment.Id);
    }

    [Fact]
    public async Task GetRepliesAsync_ReturnsRepliesForParentComment()
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
            CommentDate = DateTime.UtcNow
        };
        _db.SocialNetworkComments.Add(reply);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetRepliesAsync(parentCommentId, ct: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(reply.Id);
    }
}

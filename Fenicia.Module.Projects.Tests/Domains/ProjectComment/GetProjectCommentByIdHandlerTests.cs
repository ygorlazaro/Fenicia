using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class GetProjectCommentByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectCommentByIdHandler handler;

    public GetProjectCommentByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectCommentByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectCommentExists_ReturnsProjectCommentResponse()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new ProjectCommentModel
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.Add(comment);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectCommentByIdQuery(commentId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal(comment.Content, result.Content);
    }

    [Fact]
    public async Task Handle_WhenProjectCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectCommentByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectCommentByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectComments_ReturnsOnlyRequestedComment()
    {
        // Arrange
        var comment1Id = Guid.NewGuid();
        var comment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment1 = new ProjectCommentModel
        {
            Id = comment1Id,
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        var comment2 = new ProjectCommentModel
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.AddRange(comment1, comment2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectCommentByIdQuery(comment1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment1Id, result.Id);
        Assert.Equal(comment1.Content, result.Content);
    }

    [Fact]
    public async Task Handle_WithLongContent_ReturnsCorrectResponse()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var longContent = faker.Lorem.Paragraphs(5);
        var comment = new ProjectCommentModel
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = longContent
        };

        db.ProjectComments.Add(comment);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectCommentByIdQuery(commentId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal(longContent, result.Content);
    }
}

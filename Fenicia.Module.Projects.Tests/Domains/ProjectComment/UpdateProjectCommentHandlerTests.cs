using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class UpdateProjectCommentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateProjectCommentHandler handler;

    public UpdateProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateProjectCommentHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectCommentExists_UpdatesProjectCommentAndReturnsResponse()
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
            Content = "Old comment content"
        };

        db.ProjectComments.Add(comment);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(commentId, "New comment content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal("New comment content", result.Content);
    }

    [Fact]
    public async Task Handle_WhenProjectCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), "New comment content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), "New comment content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectComment()
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
            Content = "Comment 1 content"
        };

        var comment2 = new ProjectCommentModel
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = "Comment 2 content"
        };

        db.ProjectComments.AddRange(comment1, comment2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(comment1Id, "Updated Comment 1 content");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment1Id, result.Id);
        Assert.Equal("Updated Comment 1 content", result.Content);

        var updatedComment1 = await db.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var comment2InDb = await db.ProjectComments.FindAsync([comment2Id], CancellationToken.None);

        Assert.NotNull(updatedComment1);
        Assert.NotNull(comment2InDb);
        Assert.Equal("Updated Comment 1 content", updatedComment1.Content);
        Assert.Equal("Comment 2 content", comment2InDb.Content);
    }

    [Fact]
    public async Task Handle_WithLongContent_UpdatesProjectCommentSuccessfully()
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

        var longContent = faker.Lorem.Paragraphs(5);
        var command = new UpdateProjectCommentCommand(commentId, longContent);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(commentId, result.Id);
        Assert.Equal(longContent, result.Content);
    }
}

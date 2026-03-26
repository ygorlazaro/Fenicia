using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class DeleteProjectCommentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteProjectCommentHandler handler;

    public DeleteProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProjectCommentHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectCommentExists_SetsDeletedDate()
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

        var command = new DeleteProjectCommentCommand(commentId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedComment = await db.ProjectComments.FindAsync([commentId], CancellationToken.None);
        Assert.NotNull(deletedComment);
        Assert.NotNull(deletedComment.Deleted);
        Assert.InRange(deletedComment.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectCommentDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var comments = await db.ProjectComments.ToListAsync();
        Assert.Empty(comments);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var comments = await db.ProjectComments.ToListAsync();
        Assert.Empty(comments);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectComments_OnlyDeletesSpecified()
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

        var command = new DeleteProjectCommentCommand(comment1Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedComment = await db.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var notDeletedComment = await db.ProjectComments.FindAsync([comment2Id], CancellationToken.None);

        Assert.NotNull(deletedComment);
        Assert.NotNull(deletedComment.Deleted);
        Assert.NotNull(notDeletedComment);
        Assert.Null(notDeletedComment.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectComments_DeletesCorrectProjectComment()
    {
        // Arrange
        var comment1Id = Guid.NewGuid();
        var comment2Id = Guid.NewGuid();
        var comment3Id = Guid.NewGuid();
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

        var comment3 = new ProjectCommentModel
        {
            Id = comment3Id,
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.AddRange(comment1, comment2, comment3);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(comment2Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var comment1InDb = await db.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var deletedComment = await db.ProjectComments.FindAsync([comment2Id], CancellationToken.None);
        var comment3InDb = await db.ProjectComments.FindAsync([comment3Id], CancellationToken.None);

        Assert.NotNull(comment1InDb);
        Assert.NotNull(deletedComment);
        Assert.NotNull(comment3InDb);
        Assert.Null(comment1InDb.Deleted);
        Assert.NotNull(deletedComment.Deleted);
        Assert.Null(comment3InDb.Deleted);
    }
}

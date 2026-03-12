using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectComment.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class DeleteProjectCommentHandlerTests : IDisposable
{
    public DeleteProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new DeleteProjectCommentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly DeleteProjectCommentHandler handler;
    private readonly Faker faker;

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
            Content = this.faker.Lorem.Paragraph()
        };

        this.db.ProjectComments.Add(comment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(commentId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedComment = await this.db.ProjectComments.FindAsync([
                commentId
            ],
            CancellationToken.None);
        Assert.NotNull(deletedComment);
        Assert.NotNull(deletedComment.Deleted);
        Assert.InRange(deletedComment.Deleted.Value,
            beforeDelete.AddSeconds(-1),
            DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectCommentDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var comments = await this.db.ProjectComments.ToListAsync();
        Assert.Empty(comments);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var comments = await this.db.ProjectComments.ToListAsync();
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
            Content = this.faker.Lorem.Paragraph()
        };

        var comment2 = new ProjectCommentModel
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.db.ProjectComments.AddRange(comment1,
            comment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(comment1Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var deletedComment = await this.db.ProjectComments.FindAsync([
                comment1Id
            ],
            CancellationToken.None);
        var notDeletedComment = await this.db.ProjectComments.FindAsync([
                comment2Id
            ],
            CancellationToken.None);

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
            Content = this.faker.Lorem.Paragraph()
        };

        var comment2 = new ProjectCommentModel
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        var comment3 = new ProjectCommentModel
        {
            Id = comment3Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.db.ProjectComments.AddRange(comment1,
            comment2,
            comment3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(comment2Id);

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var comment1InDb = await this.db.ProjectComments.FindAsync([
                comment1Id
            ],
            CancellationToken.None);
        var deletedComment = await this.db.ProjectComments.FindAsync([
                comment2Id
            ],
            CancellationToken.None);
        var comment3InDb = await this.db.ProjectComments.FindAsync([
                comment3Id
            ],
            CancellationToken.None);

        Assert.NotNull(comment1InDb);
        Assert.NotNull(deletedComment);
        Assert.NotNull(comment3InDb);
        Assert.Null(comment1InDb.Deleted);
        Assert.NotNull(deletedComment.Deleted);
        Assert.Null(comment3InDb.Deleted);
    }
}

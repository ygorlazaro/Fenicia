using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class DeleteProjectCommentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectCommentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectCommentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectCommentExists_SetsDeletedDate()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new Common.Data.Models.ProjectComment
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(comment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(commentId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedComment = await this.context.ProjectComments.FindAsync([commentId], CancellationToken.None);
        Assert.That(deletedComment, Is.Not.Null);
        Assert.That(deletedComment.Deleted, Is.Not.Null);
        Assert.That(deletedComment.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedComment.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectCommentDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var comments = await this.context.ProjectComments.ToListAsync();
        Assert.That(comments, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectCommentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var comments = await this.context.ProjectComments.ToListAsync();
        Assert.That(comments, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectComments_OnlyDeletesSpecified()
    {
        // Arrange
        var comment1Id = Guid.NewGuid();
        var comment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment1 = new Common.Data.Models.ProjectComment
        {
            Id = comment1Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        var comment2 = new Common.Data.Models.ProjectComment
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.AddRange(comment1, comment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(comment1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedComment = await this.context.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var notDeletedComment = await this.context.ProjectComments.FindAsync([comment2Id], CancellationToken.None);

        Assert.That(deletedComment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedComment.Deleted, Is.Not.Null);
            Assert.That(notDeletedComment, Is.Not.Null);
        }
        Assert.That(notDeletedComment?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectComments_DeletesCorrectProjectComment()
    {
        // Arrange
        var comment1Id = Guid.NewGuid();
        var comment2Id = Guid.NewGuid();
        var comment3Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment1 = new Common.Data.Models.ProjectComment
        {
            Id = comment1Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        var comment2 = new Common.Data.Models.ProjectComment
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        var comment3 = new Common.Data.Models.ProjectComment
        {
            Id = comment3Id,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.AddRange(comment1, comment2, comment3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectCommentCommand(comment2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var comment1InDb = await this.context.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var deletedComment = await this.context.ProjectComments.FindAsync([comment2Id], CancellationToken.None);
        var comment3InDb = await this.context.ProjectComments.FindAsync([comment3Id], CancellationToken.None);

        Assert.That(comment1InDb, Is.Not.Null);
        Assert.That(deletedComment, Is.Not.Null);
        Assert.That(comment3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(comment1InDb.Deleted, Is.Null);
            Assert.That(deletedComment.Deleted, Is.Not.Null);
            Assert.That(comment3InDb.Deleted, Is.Null);
        }
    }
}

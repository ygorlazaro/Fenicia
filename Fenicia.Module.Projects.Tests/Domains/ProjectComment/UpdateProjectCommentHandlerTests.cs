using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class UpdateProjectCommentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectCommentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectCommentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectCommentExists_UpdatesProjectCommentAndReturnsResponse()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new Common.Data.Models.ProjectCommentModel
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = "Old comment content"
        };

        this.context.ProjectComments.Add(comment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(
            commentId,
            "New comment content");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(commentId));
            Assert.That(result.Content, Is.EqualTo("New comment content"));
        }
    }

    [Test]
    public async Task Handle_WhenProjectCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommentCommand(
            Guid.NewGuid(),
            "New comment content");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommentCommand(
            Guid.NewGuid(),
            "New comment content");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectComment()
    {
        // Arrange
        var comment1Id = Guid.NewGuid();
        var comment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var comment1 = new Common.Data.Models.ProjectCommentModel
        {
            Id = comment1Id,
            TaskId = taskId,
            UserId = userId,
            Content = "Comment 1 content"
        };

        var comment2 = new Common.Data.Models.ProjectCommentModel
        {
            Id = comment2Id,
            TaskId = taskId,
            UserId = userId,
            Content = "Comment 2 content"
        };

        this.context.ProjectComments.AddRange(comment1, comment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(
            comment1Id,
            "Updated Comment 1 content");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(comment1Id));
            Assert.That(result.Content, Is.EqualTo("Updated Comment 1 content"));
        }

        var updatedComment1 = await this.context.ProjectComments.FindAsync([comment1Id], CancellationToken.None);
        var comment2InDb = await this.context.ProjectComments.FindAsync([comment2Id], CancellationToken.None);

        Assert.That(updatedComment1, Is.Not.Null);
        Assert.That(comment2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedComment1.Content, Is.EqualTo("Updated Comment 1 content"));
            Assert.That(comment2InDb.Content, Is.EqualTo("Comment 2 content"));
        }
    }

    [Test]
    public async Task Handle_WithLongContent_UpdatesProjectCommentSuccessfully()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = new Common.Data.Models.ProjectCommentModel
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(comment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var longContent = this.faker.Lorem.Paragraphs(5);
        var command = new UpdateProjectCommentCommand(
            commentId,
            longContent);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(commentId));
            Assert.That(result.Content, Is.EqualTo(longContent));
        }
    }
}

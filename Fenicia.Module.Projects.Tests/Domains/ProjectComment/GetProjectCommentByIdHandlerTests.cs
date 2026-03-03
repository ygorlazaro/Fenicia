using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class GetProjectCommentByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectCommentByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectCommentByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectCommentExists_ReturnsProjectCommentResponse()
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

        var query = new GetProjectCommentByIdQuery(commentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(commentId));
            Assert.That(result.Content, Is.EqualTo(comment.Content));
        }
    }

    [Test]
    public async Task Handle_WhenProjectCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectCommentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectCommentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectComments_ReturnsOnlyRequestedComment()
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

        var query = new GetProjectCommentByIdQuery(comment1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(comment1Id));
            Assert.That(result.Content, Is.EqualTo(comment1.Content));
        }
    }

    [Test]
    public async Task Handle_WithLongContent_ReturnsCorrectResponse()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var longContent = this.faker.Lorem.Paragraphs(5);
        var comment = new Common.Data.Models.ProjectComment
        {
            Id = commentId,
            TaskId = taskId,
            UserId = userId,
            Content = longContent
        };

        this.context.ProjectComments.Add(comment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectCommentByIdQuery(commentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(commentId));
            Assert.That(result.Content, Is.EqualTo(longContent));
        }
    }
}

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class AddProjectCommentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectCommentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectCommentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectCommentAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Content, Is.EqualTo(command.Content));
        }
    }

    [Test]
    public async Task Handle_VerifiesProjectCommentWasSaved()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var comment = await this.context.ProjectComments
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.That(comment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(comment.Content, Is.EqualTo(command.Content));
            Assert.That(comment.TaskId, Is.EqualTo(command.TaskId));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectComments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command1 = new AddProjectCommentCommand(
            Guid.NewGuid(),
            taskId,
            userId,
            this.faker.Lorem.Paragraph());

        var command2 = new AddProjectCommentCommand(
            Guid.NewGuid(),
            taskId,
            userId,
            this.faker.Lorem.Paragraph());

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var comments = await this.context.ProjectComments.ToListAsync();
        Assert.That(comments, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithLongContent_AddsProjectCommentSuccessfully()
    {
        // Arrange
        var longContent = this.faker.Lorem.Paragraphs(5);
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            longContent);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Content, Is.EqualTo(longContent));
        }
    }

    [Test]
    public async Task Handle_WithShortContent_AddsProjectCommentSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Short comment");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Content, Is.EqualTo("Short comment"));
        }
    }
}

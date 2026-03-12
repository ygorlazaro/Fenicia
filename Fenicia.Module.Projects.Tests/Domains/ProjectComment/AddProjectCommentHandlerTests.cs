using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class AddProjectCommentHandlerTests : IDisposable
{
    public AddProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new AddProjectCommentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly AddProjectCommentHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectCommentAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Equal(command.Content,
            result.Content);
    }

    [Fact]
    public async Task Handle_VerifiesProjectCommentWasSaved()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var comment = await this.db.ProjectComments
            .FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.NotNull(comment);
        Assert.Equal(command.Content,
            comment.Content);
        Assert.Equal(command.TaskId,
            comment.TaskId);
    }

    [Fact]
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
        await this.handler.Handle(command1,
            CancellationToken.None);
        await this.handler.Handle(command2,
            CancellationToken.None);

        // Assert
        var comments = await this.db.ProjectComments.ToListAsync();
        Assert.Equal(2,
            comments.Count);
    }

    [Fact]
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
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Equal(longContent,
            result.Content);
    }

    [Fact]
    public async Task Handle_WithShortContent_AddsProjectCommentSuccessfully()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Short comment");

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Equal("Short comment",
            result.Content);
    }
}

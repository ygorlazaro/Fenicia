using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class AddProjectCommentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddProjectCommentHandler handler;

    public AddProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddProjectCommentHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectCommentAndReturnsResponse()
    {

        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Paragraph());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Content, result.Content);
    }

    [Fact]
    public async Task Handle_VerifiesProjectCommentWasSaved()
    {

        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Paragraph());

        await handler.Handle(command, CancellationToken.None);

        var comment = await db.ProjectComments.FirstOrDefaultAsync(c => c.Id == command.Id);

        Assert.NotNull(comment);
        Assert.Equal(command.Content, comment.Content);
        Assert.Equal(command.TaskId, comment.TaskId);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectComments()
    {

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command1 = new AddProjectCommentCommand(Guid.NewGuid(), taskId, userId, faker.Lorem.Paragraph());

        var command2 = new AddProjectCommentCommand(Guid.NewGuid(), taskId, userId, faker.Lorem.Paragraph());

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var comments = await db.ProjectComments.ToListAsync();
        Assert.Equal(2, comments.Count);
    }

    [Fact]
    public async Task Handle_WithLongContent_AddsProjectCommentSuccessfully()
    {

        var longContent = faker.Lorem.Paragraphs(5);
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), longContent);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(longContent, result.Content);
    }

    [Fact]
    public async Task Handle_WithShortContent_AddsProjectCommentSuccessfully()
    {

        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Short comment");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal("Short comment", result.Content);
    }
}

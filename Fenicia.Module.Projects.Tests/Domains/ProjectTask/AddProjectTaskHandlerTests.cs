using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class AddProjectTaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddProjectTaskHandler handler;

    public AddProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new AddProjectTaskHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectTaskAndReturnsResponse()
    {

        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Medium", "Task", faker.Random.Number(0, 100), faker.Random.Number(1, 10), DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Title, result.Title);
    }

    [Fact]
    public async Task Handle_VerifiesProjectTaskWasSaved()
    {

        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Medium", "Task", 1, 5, DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        await handler.Handle(command, CancellationToken.None);

        var task = await db.ProjectTasks.FirstOrDefaultAsync(t => t.Id == command.Id);

        Assert.NotNull(task);
        Assert.Equal(command.Title, task.Title);
        Assert.Equal(command.Description, task.Description);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectTasks()
    {

        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var command1 = new AddProjectTaskCommand(Guid.NewGuid(), projectId, statusId, faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "High", "Task", 1, 5, DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        var command2 = new AddProjectTaskCommand(Guid.NewGuid(), projectId, statusId, faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Low", "Bug", 2, 3, null, Guid.NewGuid());

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var tasks = await db.ProjectTasks.ToListAsync();
        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task Handle_WithNullDescription_AddsProjectTaskSuccessfully()
    {

        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), null, "Medium", "Task", 1, null, null, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task Handle_WithNullDueDate_AddsProjectTaskSuccessfully()
    {

        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Medium", "Task", 1, 5, null, Guid.NewGuid());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Null(result.DueDate);
    }
}

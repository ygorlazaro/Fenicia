using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectTask.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class AddProjectTaskHandlerTests : IDisposable
{
    public AddProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new AddProjectTaskHandler(this.db);
        this.faker = new Faker();
    }

    private readonly DefaultContext db;
    private readonly AddProjectTaskHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectTaskAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            this.faker.Random.Number(0,
                100),
            this.faker.Random.Number(1,
                10),
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Equal(command.Title,
            result.Title);
    }

    [Fact]
    public async Task Handle_VerifiesProjectTaskWasSaved()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            1,
            5,
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        var task = await this.db.ProjectTasks
            .FirstOrDefaultAsync(t => t.Id == command.Id);

        Assert.NotNull(task);
        Assert.Equal(command.Title,
            task.Title);
        Assert.Equal(command.Description,
            task.Description);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var command1 = new AddProjectTaskCommand(
            Guid.NewGuid(),
            projectId,
            statusId,
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "High",
            "Task",
            1,
            5,
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        var command2 = new AddProjectTaskCommand(
            Guid.NewGuid(),
            projectId,
            statusId,
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Low",
            "Bug",
            2,
            3,
            null,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command1,
            CancellationToken.None);
        await this.handler.Handle(command2,
            CancellationToken.None);

        // Assert
        var tasks = await this.db.ProjectTasks.ToListAsync();
        Assert.Equal(2,
            tasks.Count);
    }

    [Fact]
    public async Task Handle_WithNullDescription_AddsProjectTaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            null,
            "Medium",
            "Task",
            1,
            null,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Null(result.Description);
    }

    [Fact]
    public async Task Handle_WithNullDueDate_AddsProjectTaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            1,
            5,
            null,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id,
            result.Id);
        Assert.Null(result.DueDate);
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }
}

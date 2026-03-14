using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class AddProjectSubtaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddProjectSubtaskHandler handler;

    public AddProjectSubtaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new AddProjectSubtaskHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectSubtaskAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Lorem.Sentence(5), false, this.faker.Random.Number(0, 100), null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Title, result.Title);
    }

    [Fact]
    public async Task Handle_VerifiesProjectSubtaskWasSaved()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Lorem.Sentence(5), false, 1, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var subtask = await this.db.ProjectSubtasks.FirstOrDefaultAsync(s => s.Id == command.Id);

        Assert.NotNull(subtask);
        Assert.Equal(command.Title, subtask.Title);
        Assert.Equal(command.IsCompleted, subtask.IsCompleted);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectSubtasks()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectSubtaskCommand(Guid.NewGuid(), taskId, this.faker.Lorem.Sentence(5), false, 1, null);

        var command2 = new AddProjectSubtaskCommand(Guid.NewGuid(), taskId, this.faker.Lorem.Sentence(5), true, 2, DateTime.UtcNow);

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var subtasks = await this.db.ProjectSubtasks.ToListAsync();
        Assert.Equal(2, subtasks.Count);
    }

    [Fact]
    public async Task Handle_WithIsCompletedTrue_AddsProjectSubtaskSuccessfully()
    {
        // Arrange
        var completedAt = DateTime.UtcNow;
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Lorem.Sentence(5), true, 5, completedAt);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.True(result.IsCompleted);
        Assert.Equal(completedAt, result.CompletedAt);
    }

    [Fact]
    public async Task Handle_WithNullCompletedAt_AddsProjectSubtaskSuccessfully()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), this.faker.Lorem.Sentence(5), false, 1, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Null(result.CompletedAt);
    }
}
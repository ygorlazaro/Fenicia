using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class UpdateProjectSubtaskHandlerTests : IDisposable
{
    public UpdateProjectSubtaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new UpdateProjectSubtaskHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly UpdateProjectSubtaskHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectSubtaskExists_UpdatesProjectSubtaskAndReturnsResponse()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = "Old Subtask Title",
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.db.ProjectSubtasks.Add(subtask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(
            subtaskId,
            taskId,
            "New Subtask Title",
            true,
            5,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtaskId,
            result.Id);
        Assert.Equal("New Subtask Title",
            result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Subtask Title",
            true,
            5,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "New Subtask Title",
            true,
            5,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectSubtask()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = "Subtask 1 Title",
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = "Subtask 2 Title",
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.db.ProjectSubtasks.AddRange(subtask1,
            subtask2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(
            subtask1Id,
            taskId,
            "Updated Subtask 1 Title",
            true,
            10,
            DateTime.UtcNow);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtask1Id,
            result.Id);
        Assert.Equal("Updated Subtask 1 Title",
            result.Title);

        var updatedSubtask1 = await this.db.ProjectSubtasks.FindAsync([
                subtask1Id
            ],
            CancellationToken.None);
        var subtask2InDb = await this.db.ProjectSubtasks.FindAsync([
                subtask2Id
            ],
            CancellationToken.None);

        Assert.NotNull(updatedSubtask1);
        Assert.NotNull(subtask2InDb);
        Assert.Equal("Updated Subtask 1 Title",
            updatedSubtask1.Title);
        Assert.Equal("Subtask 2 Title",
            subtask2InDb.Title);
    }

    [Fact]
    public async Task Handle_WithIsCompletedChange_UpdatesProjectSubtaskSuccessfully()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.db.ProjectSubtasks.Add(subtask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var completedAt = DateTime.UtcNow;
        var command = new UpdateProjectSubtaskCommand(
            subtaskId,
            taskId,
            "Updated Title",
            true,
            3,
            completedAt);

        // Act
        var result = await this.handler.Handle(command,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtaskId,
            result.Id);
        Assert.True(result.IsCompleted);
        Assert.Equal(completedAt,
            result.CompletedAt);
    }
}

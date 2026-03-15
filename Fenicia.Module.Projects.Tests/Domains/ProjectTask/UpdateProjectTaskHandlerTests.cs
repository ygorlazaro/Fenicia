using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class UpdateProjectTaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateProjectTaskHandler handler;

    public UpdateProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateProjectTaskHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskExists_UpdatesProjectTaskAndReturnsResponse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task = new ProjectTaskModel
        {
            Id = taskId,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Old Task Title",
            Description = "Old Description",
            Priority = EnumTaskPriority.Low,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow.AddDays(5),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(task);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(taskId, projectId, statusId, "New Task Title", "New Description", "High", "Bug", 10, 8, DateTime.UtcNow.AddDays(14), Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal("New Task Title", result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "New Task Title", "New Description", "High", "Bug", 1, 5, null, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "New Task Title", "New Description", "High", "Bug", 1, 5, null, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectTask()
    {
        // Arrange
        var task1Id = Guid.NewGuid();
        var task2Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();

        var task1 = new ProjectTaskModel
        {
            Id = task1Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Task 1 Title",
            Description = "Task 1 Description",
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        var task2 = new ProjectTaskModel
        {
            Id = task2Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = "Task 2 Title",
            Description = "Task 2 Description",
            Priority = EnumTaskPriority.Low,
            Type = EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.AddRange(task1, task2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(task1Id, projectId, statusId, "Updated Task 1 Title", "Updated Task 1 Description", "High", "Task", 5, 10, DateTime.UtcNow.AddDays(21), task1.CreatedBy);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task1Id, result.Id);
        Assert.Equal("Updated Task 1 Title", result.Title);

        var updatedTask1 = await db.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var task2InDb = await db.ProjectTasks.FindAsync([task2Id], CancellationToken.None);

        Assert.NotNull(updatedTask1);
        Assert.NotNull(task2InDb);
        Assert.Equal("Updated Task 1 Title", updatedTask1.Title);
        Assert.Equal("Task 2 Title", task2InDb.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_UpdatesProjectTaskSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task = new ProjectTaskModel
        {
            Id = taskId,
            ProjectId = projectId,
            StatusId = statusId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(task);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(taskId, projectId, statusId, "Updated Title", null, "Medium", "Task", 1, 5, null, Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Null(result.Description);
    }
}

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class DeleteProjectTaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteProjectTaskHandler handler;

    public DeleteProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProjectTaskHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskExists_SetsDeletedDate()
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

        var command = new DeleteProjectTaskCommand(taskId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedTask = await db.ProjectTasks.FindAsync([taskId], CancellationToken.None);
        Assert.NotNull(deletedTask);
        Assert.NotNull(deletedTask.Deleted);
        Assert.InRange(deletedTask.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectTaskDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var tasks = await db.ProjectTasks.ToListAsync();
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectTaskCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var tasks = await db.ProjectTasks.ToListAsync();
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTasks_OnlyDeletesSpecified()
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
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
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
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Low,
            Type = EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.AddRange(task1, task2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskCommand(task1Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedTask = await db.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var notDeletedTask = await db.ProjectTasks.FindAsync([task2Id], CancellationToken.None);

        Assert.NotNull(deletedTask);
        Assert.NotNull(deletedTask.Deleted);
        Assert.NotNull(notDeletedTask);
        Assert.Null(notDeletedTask.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTasks_DeletesCorrectProjectTask()
    {
        // Arrange
        var task1Id = Guid.NewGuid();
        var task2Id = Guid.NewGuid();
        var task3Id = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();

        var task1 = new ProjectTaskModel
        {
            Id = task1Id,
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

        var task2 = new ProjectTaskModel
        {
            Id = task2Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Low,
            Type = EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        var task3 = new ProjectTaskModel
        {
            Id = task3Id,
            ProjectId = projectId,
            StatusId = statusId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.High,
            Type = EnumTaskType.Task,
            Order = 3,
            EstimatePoints = 8,
            DueDate = DateTime.UtcNow.AddDays(14),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.AddRange(task1, task2, task3);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectTaskCommand(task2Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var task1InDb = await db.ProjectTasks.FindAsync([task1Id], CancellationToken.None);
        var deletedTask = await db.ProjectTasks.FindAsync([task2Id], CancellationToken.None);
        var task3InDb = await db.ProjectTasks.FindAsync([task3Id], CancellationToken.None);

        Assert.NotNull(task1InDb);
        Assert.NotNull(deletedTask);
        Assert.NotNull(task3InDb);
        Assert.Null(task1InDb.Deleted);
        Assert.NotNull(deletedTask.Deleted);
        Assert.Null(task3InDb.Deleted);
    }
}

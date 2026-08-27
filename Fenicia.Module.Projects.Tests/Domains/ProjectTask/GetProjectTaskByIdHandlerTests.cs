using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class GetProjectTaskByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectTaskByIdHandler handler;

    public GetProjectTaskByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectTaskByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskExists_ReturnsProjectTaskResponse()
    {

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
            Priority = EnumTaskPriority.High,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(task);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal(task.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskDoesNotExist_ReturnsNull()
    {

        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTasks_ReturnsOnlyRequestedTask()
    {

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
            Priority = EnumTaskPriority.High,
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
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.AddRange(task1, task2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(task1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(task1Id, result.Id);
        Assert.Equal(task1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
    {

        var taskId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task = new ProjectTaskModel
        {
            Id = taskId,
            ProjectId = projectId,
            StatusId = statusId,
            Title = faker.Lorem.Sentence(5),
            Description = null,
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = null,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(task);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Null(result.Description);
    }
}

using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class GetAllProjectTaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectTaskHandler handler;

    public GetAllProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProjectTaskHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllProjectTaskQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectTasks_ReturnsAllProjectTasks()
    {

        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task1 = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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

        var query = new GetAllProjectTaskQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(task1.Id, result[0].Id);
        Assert.Equal(task2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                Description = faker.Lorem.Paragraph(),
                Priority = EnumTaskPriority.Medium,
                Type = EnumTaskType.Task,
                Order = i,
                EstimatePoints = faker.Random.Number(1, 10),
                DueDate = DateTime.UtcNow.AddDays(i),
                CreatedBy = Guid.NewGuid()
            };
            db.ProjectTasks.Add(task);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                Description = faker.Lorem.Paragraph(),
                Priority = EnumTaskPriority.Medium,
                Type = EnumTaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            db.ProjectTasks.Add(task);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                Description = faker.Lorem.Paragraph(),
                Priority = EnumTaskPriority.Medium,
                Type = EnumTaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            db.ProjectTasks.Add(task);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}

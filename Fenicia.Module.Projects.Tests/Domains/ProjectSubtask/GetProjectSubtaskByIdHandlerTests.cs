using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class GetProjectSubtaskByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectSubtaskByIdHandler handler;

    public GetProjectSubtaskByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectSubtaskByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectSubtaskExists_ReturnsProjectSubtaskResponse()
    {

        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        db.ProjectSubtasks.Add(subtask);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(subtaskId, result.Id);
        Assert.Equal(subtask.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_ReturnsNull()
    {

        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectSubtasks_ReturnsOnlyRequestedSubtask()
    {

        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        db.ProjectSubtasks.AddRange(subtask1, subtask2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtask1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(subtask1Id, result.Id);
        Assert.Equal(subtask1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithCompletedSubtask_ReturnsCorrectResponse()
    {

        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var completedAt = DateTime.UtcNow.AddDays(-5);
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 1,
            CompletedAt = completedAt
        };

        db.ProjectSubtasks.Add(subtask);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(subtaskId, result.Id);
        Assert.True(result.IsCompleted);
        Assert.Equal(completedAt, result.CompletedAt);
    }
}

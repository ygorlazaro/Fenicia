using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class GetAllProjectSubtaskHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectSubtaskHandler handler;

    public GetAllProjectSubtaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProjectSubtaskHandler(db);
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
        // Arrange
        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectSubtasks_ReturnsAllProjectSubtasks()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var subtask1 = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            Title = faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        db.ProjectSubtasks.AddRange(subtask1, subtask2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(subtask1.Id, result[0].Id);
        Assert.Equal(subtask2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var subtask = new ProjectSubtaskModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                IsCompleted = i % 2 == 0,
                Order = i,
                CompletedAt = i % 2 == 0 ? DateTime.UtcNow.AddDays(-i) : null
            };
            db.ProjectSubtasks.Add(subtask);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery(2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var subtask = new ProjectSubtaskModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                IsCompleted = false,
                Order = i,
                CompletedAt = null
            };
            db.ProjectSubtasks.Add(subtask);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery(10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var subtask = new ProjectSubtaskModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                Title = $"{faker.Lorem.Sentence(5)} {i}",
                IsCompleted = false,
                Order = i,
                CompletedAt = null
            };
            db.ProjectSubtasks.Add(subtask);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectSubtaskQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}

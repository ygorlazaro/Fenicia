using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class GetProjectSubtaskByIdHandlerTests : IDisposable
{
    public GetProjectSubtaskByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new GetProjectSubtaskByIdHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetProjectSubtaskByIdHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectSubtaskExists_ReturnsProjectSubtaskResponse()
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

        this.context.ProjectSubtasks.Add(subtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtaskId, result.Id);
        Assert.Equal(subtask.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectSubtaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectSubtasks_ReturnsOnlyRequestedSubtask()
    {
        // Arrange
        var subtask1Id = Guid.NewGuid();
        var subtask2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var subtask1 = new ProjectSubtaskModel
        {
            Id = subtask1Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var subtask2 = new ProjectSubtaskModel
        {
            Id = subtask2Id,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow.AddDays(-2)
        };

        this.context.ProjectSubtasks.AddRange(subtask1, subtask2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtask1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtask1Id, result.Id);
        Assert.Equal(subtask1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithCompletedSubtask_ReturnsCorrectResponse()
    {
        // Arrange
        var subtaskId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var completedAt = DateTime.UtcNow.AddDays(-5);
        var subtask = new ProjectSubtaskModel
        {
            Id = subtaskId,
            TaskId = taskId,
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 1,
            CompletedAt = completedAt
        };

        this.context.ProjectSubtasks.Add(subtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectSubtaskByIdQuery(subtaskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subtaskId, result.Id);
        Assert.True(result.IsCompleted);
        Assert.Equal(completedAt, result.CompletedAt);
    }
}

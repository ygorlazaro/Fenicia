using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Module.Projects.Domains.ProjectTask.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class GetProjectTaskByIdHandlerTests : IDisposable
{
    public GetProjectTaskByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new GetProjectTaskByIdHandler(this.context);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetProjectTaskByIdHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenProjectTaskExists_ReturnsProjectTaskResponse()
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.High,
            Type = Common.Enums.Project.EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal(task.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WhenProjectTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectTaskByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectTasks_ReturnsOnlyRequestedTask()
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.High,
            Type = Common.Enums.Project.EnumTaskType.Task,
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 3,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(task1, task2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(task1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(task1Id, result.Id);
        Assert.Equal(task1.Title, result.Title);
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsCorrectResponse()
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
            Title = this.faker.Lorem.Sentence(5),
            Description = null,
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Task,
            Order = 1,
            EstimatePoints = null,
            DueDate = null,
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(task);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectTaskByIdQuery(taskId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Null(result.Description);
    }
}

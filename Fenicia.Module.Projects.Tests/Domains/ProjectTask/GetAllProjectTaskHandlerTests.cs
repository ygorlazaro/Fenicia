using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class GetAllProjectTaskHandlerTests : IDisposable
{
    public GetAllProjectTaskHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.handler = new GetAllProjectTaskHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly GetAllProjectTaskHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectTasks_ReturnsAllProjectTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var task1 = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
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
            Id = Guid.NewGuid(),
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

        this.db.ProjectTasks.AddRange(task1,
            task2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2,
            result.Count);
        Assert.Equal(task1.Id,
            result[0].Id);
        Assert.Equal(task2.Id,
            result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.EnumTaskPriority.Medium,
                Type = Common.Enums.Project.EnumTaskType.Task,
                Order = i,
                EstimatePoints = this.faker.Random.Number(1,
                    10),
                DueDate = DateTime.UtcNow.AddDays(i),
                CreatedBy = Guid.NewGuid()
            };
            this.db.ProjectTasks.Add(task);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(2);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.EnumTaskPriority.Medium,
                Type = Common.Enums.Project.EnumTaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            this.db.ProjectTasks.Add(task);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery(10);

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var task = new ProjectTaskModel
            {
                Id = Guid.NewGuid(),
                ProjectId = projectId,
                StatusId = statusId,
                Title = $"{this.faker.Lorem.Sentence(5)} {i}",
                Description = this.faker.Lorem.Paragraph(),
                Priority = Common.Enums.Project.EnumTaskPriority.Medium,
                Type = Common.Enums.Project.EnumTaskType.Task,
                Order = i,
                EstimatePoints = 5,
                DueDate = null,
                CreatedBy = Guid.NewGuid()
            };
            this.db.ProjectTasks.Add(task);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectTaskQuery();

        // Act
        var result = await this.handler.Handle(query,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10,
            result.Count);
    }
}

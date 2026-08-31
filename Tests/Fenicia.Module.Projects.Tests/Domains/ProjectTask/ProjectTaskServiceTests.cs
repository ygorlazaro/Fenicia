using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectTaskService _service;
    private readonly Guid _companyId;

    public ProjectTaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ProjectTaskService(new ProjectTaskRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenTasksExist_ReturnsPaginationWithTasks()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Sentence(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectTaskQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsTask()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Sentence(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectTaskByIdQuery(projectTask.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(projectTask.Id);
        result.Title.Should().Be(projectTask.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectTaskByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesTask()
    {
        // Arrange
        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence(), _faker.Lorem.Sentence(), "Medium", "Task", 1, 3, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskExists_UpdatesTask()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Sentence(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, "Updated Title", projectTask.Description, "High", "Bug", 2, 5, DateTime.UtcNow, projectTask.CreatedBy);

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(projectTask.Id);
        result.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Updated Title", _faker.Lorem.Sentence(), "High", "Bug", 2, 5, DateTime.UtcNow, Guid.NewGuid());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskExists_SoftDeletesTask()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            Description = _faker.Lorem.Sentence(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 3,
            DueDate = DateTime.UtcNow,
            CreatedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectTaskCommand(projectTask.Id), CancellationToken.None);

        // Assert
        var deletedTask = await _db.ProjectTasks.IgnoreQueryFilters().FirstOrDefaultAsync(pt => pt.Id == projectTask.Id);
        deletedTask.Should().NotBeNull();
        deletedTask!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectTaskCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.ProjectTasks.CountAsync();
        count.Should().Be(0);
    }
}

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IProjectTaskRepository> _mockRepository;
    private readonly ProjectTaskService _service;

    public ProjectTaskServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IProjectTaskRepository>();
        _service = new ProjectTaskService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenTasksExist_ReturnsTasks()
    {
        var tasks = new List<ProjectTaskModel>
        {
            new()
            {
                Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), StatusId = Guid.NewGuid(),
                Title = _faker.Lorem.Sentence(), Priority = EnumTaskPriority.Medium, Type = EnumTaskType.Task,
                Order = 1, CreatedBy = Guid.NewGuid(), CompanyId = Guid.NewGuid()
            }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<ProjectTaskModel>(tasks));

        var result = await _service.GetAllAsync(new GetAllProjectTaskQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(tasks[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsTask()
    {
        var task = new ProjectTaskModel
        {
            Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), StatusId = Guid.NewGuid(), Title = "T",
            Priority = EnumTaskPriority.Medium, Type = EnumTaskType.Task, Order = 1, CreatedBy = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByIdWithRelationsAsync(task.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var result = await _service.GetByIdAsync(new GetProjectTaskByIdQuery(task.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(task.Id);
        result.Title.Should().Be("T");
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdWithRelationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectTaskModel?)null);

        var result = await _service.GetByIdAsync(new GetProjectTaskByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedTask()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "T",
            null,
            nameof(EnumTaskPriority.Medium),
            nameof(EnumTaskType.Task),
            1,
            null,
            null,
            Guid.NewGuid());

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProjectTaskModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectTaskModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskExists_ReturnsUpdatedTask()
    {
        var task = new ProjectTaskModel
        {
            Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), StatusId = Guid.NewGuid(), Title = "U",
            Priority = EnumTaskPriority.High, Type = EnumTaskType.Bug, Order = 2, CreatedBy = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.UpdateAsync(task.Id, It.IsAny<ProjectTaskModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var command = new UpdateProjectTaskCommand(
            task.Id,
            task.ProjectId,
            task.StatusId,
            "U",
            null,
            nameof(EnumTaskPriority.High),
            nameof(EnumTaskType.Bug),
            2,
            null,
            null,
            task.CreatedBy);

        var result = await _service.UpdateAsync(command, task.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("U");
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Guid>(),
                It.IsAny<ProjectTaskModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectTaskModel?)null);

        var command = new UpdateProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "U",
            null,
            nameof(EnumTaskPriority.Medium),
            nameof(EnumTaskType.Task),
            1,
            null,
            null,
            Guid.NewGuid());

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectTaskCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
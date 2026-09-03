using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class ProjectSubtaskServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<ProjectSubtaskModel>> _mockRepository;
    private readonly ProjectSubtaskService _service;

    public ProjectSubtaskServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IRepository<ProjectSubtaskModel>>();
        _service = new ProjectSubtaskService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenSubtasksExist_ReturnsSubtasks()
    {
        var subtasks = new List<ProjectSubtaskModel>
        {
            new()
            {
                Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), Title = _faker.Lorem.Sentence(), IsCompleted = false,
                Order = 1, CompanyId = Guid.NewGuid()
            }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<ProjectSubtaskModel>(subtasks));

        var result = await _service.GetAllAsync(new GetAllProjectSubtaskQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(subtasks[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskExists_ReturnsSubtask()
    {
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), Title = "S", IsCompleted = false, Order = 1,
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByIdAsync(subtask.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subtask);

        var result = await _service.GetByIdAsync(new GetProjectSubtaskByIdQuery(subtask.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("S");
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectSubtaskModel?)null);

        var result = await _service.GetByIdAsync(
            new GetProjectSubtaskByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedSubtask()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "S", false, 1, null);

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProjectSubtaskModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectSubtaskModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtaskExists_ReturnsUpdatedSubtask()
    {
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), Title = "U", IsCompleted = true, Order = 2,
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.UpdateAsync(
                subtask.Id,
                It.IsAny<ProjectSubtaskModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(subtask);

        var command = new UpdateProjectSubtaskCommand(subtask.Id, subtask.TaskId, "U", true, 2, DateTime.UtcNow);

        var result = await _service.UpdateAsync(command, subtask.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Title.Should().Be("U");
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtaskDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(
                It.IsAny<Guid>(),
                It.IsAny<ProjectSubtaskModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectSubtaskModel?)null);

        var command = new UpdateProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "U", true, 2, DateTime.UtcNow);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectSubtaskCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
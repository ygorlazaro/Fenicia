using AwesomeAssertions;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeServiceTests
{
    private readonly Mock<IRepository<TaskAssigneeModel>> _mockRepository;
    private readonly ProjectTaskAssigneeService _service;

    public ProjectTaskAssigneeServiceTests()
    {
        _mockRepository = new Mock<IRepository<TaskAssigneeModel>>();
        _service = new ProjectTaskAssigneeService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenAssigneesExist_ReturnsAssignees()
    {
        var assignees = new List<TaskAssigneeModel>
        {
            new() { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Role = EnumAssigneeRole.Owner, AssignedAt = DateTime.UtcNow, CompanyId = Guid.NewGuid() }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<TaskAssigneeModel>(assignees));

        var result = await _service.GetAllAsync(new GetAllProjectTaskAssigneeQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(assignees[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeExists_ReturnsAssignee()
    {
        var assignee = new TaskAssigneeModel { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Role = EnumAssigneeRole.Owner, AssignedAt = DateTime.UtcNow, CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.GetByIdAsync(assignee.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignee);

        var result = await _service.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(assignee.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(assignee.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskAssigneeModel?)null);

        var result = await _service.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedAssignee()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<TaskAssigneeModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskAssigneeModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenAssigneeExists_ReturnsUpdatedAssignee()
    {
        var assignee = new TaskAssigneeModel { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Role = EnumAssigneeRole.Contributor, AssignedAt = DateTime.UtcNow, CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.UpdateAsync(assignee.Id, It.IsAny<TaskAssigneeModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignee);

        var command = new UpdateProjectTaskAssigneeCommand(assignee.Id, assignee.TaskId, assignee.UserId, "Contributor", DateTime.UtcNow);

        var result = await _service.UpdateAsync(command, assignee.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Role.Should().Be("Contributor");
    }

    [Fact]
    public async Task UpdateAsync_WhenAssigneeDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<TaskAssigneeModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskAssigneeModel?)null);

        var command = new UpdateProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectTaskAssigneeCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
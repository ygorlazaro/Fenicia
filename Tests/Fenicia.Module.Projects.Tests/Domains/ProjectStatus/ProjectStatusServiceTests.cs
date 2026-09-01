using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class ProjectStatusServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IProjectStatusRepository> _mockRepository;
    private readonly ProjectStatusService _service;

    public ProjectStatusServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IProjectStatusRepository>();
        _service = new ProjectStatusService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatusesExist_ReturnsStatuses()
    {
        var statuses = new List<ProjectStatusModel>
        {
            new() { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), Color = "#FF0000", Order = 1, IsFinal = false, CompanyId = Guid.NewGuid() }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<ProjectStatusModel>(statuses));

        var result = await _service.GetAllAsync(new GetAllProjectStatusQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(statuses[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusExists_ReturnsStatus()
    {
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "Active", Color = "#FF0000", Order = 1, IsFinal = false, CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.GetByIdAsync(status.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var result = await _service.GetByIdAsync(new GetProjectStatusByIdQuery(status.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Active");
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectStatusModel?)null);

        var result = await _service.GetByIdAsync(new GetProjectStatusByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedStatus()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "Active", "#FF0000", 1, false);

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProjectStatusModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectStatusModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenStatusExists_ReturnsUpdatedStatus()
    {
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), ProjectId = Guid.NewGuid(), Name = "Done", Color = "#00FF00", Order = 2, IsFinal = true, CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.UpdateAsync(status.Id, It.IsAny<ProjectStatusModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new UpdateProjectStatusCommand(status.Id, status.ProjectId, "Done", "#00FF00", 2, true);

        var result = await _service.UpdateAsync(command, status.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenStatusDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ProjectStatusModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectStatusModel?)null);

        var command = new UpdateProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "Done", "#00FF00", 2, true);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectStatusCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
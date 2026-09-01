using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IProjectRepository> _mockRepository;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IProjectRepository>();
        _service = new ProjectService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsProjects()
    {
        var projects = new List<ProjectModel>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = _faker.Commerce.Categories(1).First(),
                Description = _faker.Commerce.ProductDescription(),
                Status = EnumProjectStatus.Active,
                Owner = Guid.NewGuid(),
                CompanyId = Guid.NewGuid()
            }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<ProjectModel>(projects));

        var result = await _service.GetAllAsync(new GetAllProjectQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(projects[0].Id);
        result.First().Title.Should().Be(projects[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsProject()
    {
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByIdWithRelationsAsync(project.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _service.GetByIdAsync(new GetProjectByIdQuery(project.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Title.Should().Be(project.Title);
        result.Statuses.Should().NotBeNull();
        result.Tasks.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdWithRelationsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectModel?)null);

        var result = await _service.GetByIdAsync(new GetProjectByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesProject()
    {
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Draft),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            Guid.NewGuid());
        var companyId = Guid.NewGuid();

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Title.Should().Be(command.Title);
        result.Status.Should().Be(nameof(EnumProjectStatus.Draft));
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectExists_UpdatesProject()
    {
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = "Updated Title",
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.UpdateAsync(project.Id, It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var command = new UpdateProjectCommand(
            project.Id,
            project.Title,
            project.Description,
            nameof(EnumProjectStatus.Active),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            project.Owner);

        var result = await _service.UpdateAsync(command, project.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<ProjectModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectModel?)null);

        var command = new UpdateProjectCommand(
            Guid.NewGuid(),
            "Updated Title",
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Active),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
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

        await _service.DeleteAsync(new DeleteProjectCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
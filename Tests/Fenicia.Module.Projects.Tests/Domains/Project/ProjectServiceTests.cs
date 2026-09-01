using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectService _service;
    private readonly Guid _companyId;

    public ProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ProjectService(new ProjectRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenProjectsExist_ReturnsPaginationWithProjects()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectQuery(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(project.Id);
        result.First().Title.Should().Be(project.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsProject()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectByIdQuery(project.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Title.Should().Be(project.Title);
        result.Statuses.Should().NotBeNull();
        result.Tasks.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesProject()
    {
        // Arrange
        var command = new AddProjectCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First(), _faker.Commerce.ProductDescription(), nameof(EnumProjectStatus.Draft), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Title.Should().Be(command.Title);
        result.Status.Should().Be(nameof(EnumProjectStatus.Draft));
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectExists_UpdatesProject()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(project.Id, "Updated Title", _faker.Commerce.ProductDescription(), nameof(EnumProjectStatus.Active), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), project.Owner);

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(project.Id);
        result.Title.Should().Be("Updated Title");
        result.Status.Should().Be(nameof(EnumProjectStatus.Active));
    }

    [Fact]
    public async Task UpdateAsync_WhenProjectDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommand(Guid.NewGuid(), "Updated Title", _faker.Commerce.ProductDescription(), nameof(EnumProjectStatus.Active), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), Guid.NewGuid());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_SoftDeletesProject()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        var deletedProject = await _db.Projects.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == project.Id);
        deletedProject.Should().NotBeNull();
        deletedProject.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.Projects.CountAsync();
        count.Should().Be(0);
    }
}

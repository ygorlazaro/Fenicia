using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class ProjectSubtaskServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectSubtaskService _service;
    private readonly Guid _companyId;

    public ProjectSubtaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ProjectSubtaskService(new ProjectSubtaskRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenSubtasksExist_ReturnsPaginationWithSubtasks()
    {
        // Arrange
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(projectSubtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectSubtaskQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskExists_ReturnsSubtask()
    {
        // Arrange
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(projectSubtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectSubtaskByIdQuery(projectSubtask.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(projectSubtask.Id);
        result.Title.Should().Be(projectSubtask.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectSubtaskByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesSubtask()
    {
        // Arrange
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence(), false, 1, null);

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtaskExists_UpdatesSubtask()
    {
        // Arrange
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(projectSubtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(projectSubtask.Id, projectSubtask.TaskId, "Updated Title", true, 2, DateTime.UtcNow);

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(projectSubtask.Id);
        result.Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateAsync_WhenSubtaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "Updated Title", true, 2, DateTime.UtcNow);

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSubtaskExists_SoftDeletesSubtask()
    {
        // Arrange
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(projectSubtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectSubtaskCommand(projectSubtask.Id), CancellationToken.None);

        // Assert
        var deletedSubtask = await _db.ProjectSubtasks.IgnoreQueryFilters().FirstOrDefaultAsync(ps => ps.Id == projectSubtask.Id);
        deletedSubtask.Should().NotBeNull();
        deletedSubtask!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSubtaskDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectSubtaskCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.ProjectSubtasks.CountAsync();
        count.Should().Be(0);
    }
}

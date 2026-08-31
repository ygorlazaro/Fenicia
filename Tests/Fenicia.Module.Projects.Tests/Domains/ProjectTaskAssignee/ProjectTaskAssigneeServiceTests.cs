using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectTaskAssigneeService _service;
    private readonly Guid _companyId;

    public ProjectTaskAssigneeServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ProjectTaskAssigneeService(new ProjectTaskAssigneeRepository(_db));
        _companyId = companyContext.CompanyId;
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenAssigneesExist_ReturnsPaginationWithAssignees()
    {
        // Arrange
        var assignee = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = _faker.Date.Past(),
            CompanyId = _companyId
        };
        _db.ProjectTaskAssignees.Add(assignee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectTaskAssigneeQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeExists_ReturnsAssignee()
    {
        // Arrange
        var assignee = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = _faker.Date.Past(),
            CompanyId = _companyId
        };
        _db.ProjectTaskAssignees.Add(assignee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(assignee.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(assignee.Id);
        result.TaskId.Should().Be(assignee.TaskId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectTaskAssigneeByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesAssignee()
    {
        // Arrange
        var command = new AddProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past());

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.TaskId.Should().Be(command.TaskId);
    }

    [Fact]
    public async Task UpdateAsync_WhenAssigneeExists_UpdatesAssignee()
    {
        // Arrange
        var assignee = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = _faker.Date.Past(),
            CompanyId = _companyId
        };
        _db.ProjectTaskAssignees.Add(assignee);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskAssigneeCommand(assignee.Id, assignee.TaskId, assignee.UserId, "Contributor", _faker.Date.Past());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(assignee.Id);
        result.Role.Should().Be("Contributor");
    }

    [Fact]
    public async Task UpdateAsync_WhenAssigneeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAssigneeExists_SoftDeletesAssignee()
    {
        // Arrange
        var assignee = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = _faker.Date.Past(),
            CompanyId = _companyId
        };
        _db.ProjectTaskAssignees.Add(assignee);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectTaskAssigneeCommand(assignee.Id), CancellationToken.None);

        // Assert
        var deletedAssignee = await _db.ProjectTaskAssignees.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == assignee.Id);
        deletedAssignee.Should().NotBeNull();
        deletedAssignee!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAssigneeDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectTaskAssigneeCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.ProjectTaskAssignees.CountAsync();
        count.Should().Be(0);
    }
}

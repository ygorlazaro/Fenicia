using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeControllerTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly ProjectTaskAssigneeController _controller;
    private readonly Guid _companyId;

    public ProjectTaskAssigneeControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectTaskAssigneeRepository(_db);
        var service = new ProjectTaskAssigneeService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectTaskAssigneeController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _companyId = companyContext.CompanyId;
        _faker = new Faker();
        SetupUserClaims(Guid.NewGuid());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenAssigneesExist_ReturnsOk()
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

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
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

        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(assignee.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as GetProjectTaskAssigneeByIdResponse;
        response.Should().NotBeNull();
        response.Id.Should().Be(assignee.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_CreatesAssignee()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new AddProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past());

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = result.Result as CreatedResult;
        createdResult!.Value.Should().BeOfType<AddProjectTaskAssigneeResponse>();
    }

    [Fact]
    public async Task PatchAsync_WhenAssigneeExists_UpdatesAssignee()
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

        var wide = new WideEventContext();
        var command = new UpdateProjectTaskAssigneeCommand(assignee.Id, assignee.TaskId, assignee.UserId, "Contributor", _faker.Date.Past());

        // Act
        var result = await _controller.PatchAsync(command, assignee.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as UpdateProjectTaskAssigneeResponse;
        response.Should().NotBeNull();
        response.Role.Should().Be("Contributor");
    }

    [Fact]
    public async Task PatchAsync_WhenAssigneeDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenAssigneeExists_ReturnsNoContent()
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

        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(assignee.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenAssigneeDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

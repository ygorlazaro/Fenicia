using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeControllerTests
{
    private readonly ProjectTaskAssigneeController _controller;
    private readonly Faker _faker;
    private readonly Mock<IProjectTaskAssigneeService> _mockService;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ProjectTaskAssigneeControllerTests()
    {
        _mockService = new Mock<IProjectTaskAssigneeService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectTaskAssigneeController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenAssigneesExist_ReturnsOkWithAssignees()
    {
        var wide = new WideEventContext();
        var assignees = new List<GetAllProjectTaskAssigneeResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past(), Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectTaskAssigneeQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignees);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var assignee = new GetProjectTaskAssigneeByIdResponse(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", _faker.Date.Past(), Guid.NewGuid());

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectTaskAssigneeByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(assignee);

        var result = await _controller.GetByIdAsync(assignee.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAssigneeDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectTaskAssigneeByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectTaskAssigneeByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);
        var response = new AddProjectTaskAssigneeResponse(command.Id, command.TaskId, command.UserId, command.Role, command.AssignedAt, Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAssigneeExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var assigneeId = Guid.NewGuid();
        var command = new UpdateProjectTaskAssigneeCommand(assigneeId, Guid.NewGuid(), Guid.NewGuid(), "Contributor", DateTime.UtcNow);
        var response = new UpdateProjectTaskAssigneeResponse(command.Id, command.TaskId, command.UserId, command.Role, command.AssignedAt, Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectTaskAssigneeCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, assigneeId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAssigneeDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectTaskAssigneeCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectTaskAssigneeResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectTaskAssigneeCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteAsync(id, wide, CancellationToken.None);

        result.Should().BeOfType<NoContentResult>();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _mockHttpContext.Setup(x => x.User).Returns(principal);
        _controller.ControllerContext.HttpContext!.User = principal;
    }
}
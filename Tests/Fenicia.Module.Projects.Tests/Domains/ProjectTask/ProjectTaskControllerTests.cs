using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskControllerTests
{
    private readonly ProjectTaskController _controller;
    private readonly Faker _faker;
    private readonly Mock<IProjectTaskService> _mockService;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ProjectTaskControllerTests()
    {
        _mockService = new Mock<IProjectTaskService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectTaskController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenTasksExist_ReturnsOkWithTasks()
    {
        var wide = new WideEventContext();
        var tasks = new List<GetAllProjectTaskResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _faker.Commerce.Categories(1).First(), null, nameof(EnumTaskPriority.Medium), nameof(EnumTaskType.Task), 1, null, null, _testUserId, Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectTaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tasks);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsOkWithTask()
    {
        var wide = new WideEventContext();
        var task = new GetProjectTaskByIdResponse(
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
            _testUserId,
            Guid.NewGuid(),
            [],
            [],
            [],
            []);

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectTaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        var result = await _controller.GetByIdAsync(task.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectTaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectTaskByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "T", null, nameof(EnumTaskPriority.Medium), nameof(EnumTaskType.Task), 1, null, null, _testUserId);
        var response = new AddProjectTaskResponse(command.Id, command.ProjectId, command.StatusId, command.Title, command.Description, command.Priority, command.Type, command.Order, command.EstimatePoints, command.DueDate, command.CreatedBy, Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenTaskExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var taskId = Guid.NewGuid();
        var command = new UpdateProjectTaskCommand(taskId, Guid.NewGuid(), Guid.NewGuid(), "U", null, nameof(EnumTaskPriority.High), nameof(EnumTaskType.Bug), 2, null, null, _testUserId);
        var response = new UpdateProjectTaskResponse(command.Id, command.ProjectId, command.StatusId, command.Title, command.Description, command.Priority, command.Type, command.Order, command.EstimatePoints, command.DueDate, command.CreatedBy, Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectTaskCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, taskId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "U", null, nameof(EnumTaskPriority.Medium), nameof(EnumTaskType.Task), 1, null, null, _testUserId);

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectTaskCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectTaskResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectTaskCommand>(), It.IsAny<CancellationToken>()))
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
        _controller.ControllerContext.HttpContext.User = principal;
    }
}
using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class ProjectSubtaskControllerTests
{
    private readonly ProjectSubtaskController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IProjectSubtaskService> _mockService;
    private readonly Guid _testUserId;

    public ProjectSubtaskControllerTests()
    {
        _mockService = new Mock<IProjectSubtaskService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectSubtaskController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenSubtasksExist_ReturnsOkWithSubtasks()
    {
        var wide = new WideEventContext();
        var subtasks = new List<GetAllProjectSubtaskResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence(), false, 1, null, Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectSubtaskQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(subtasks);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var subtask = new GetProjectSubtaskByIdResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "S",
            false,
            1,
            null,
            Guid.NewGuid());

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectSubtaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(subtask);

        var result = await _controller.GetByIdAsync(subtask.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectSubtaskByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectSubtaskByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "S", false, 1, null);
        var response = new AddProjectSubtaskResponse(
            command.Id,
            command.TaskId,
            command.Title,
            command.IsCompleted,
            command.Order,
            command.CompletedAt,
            Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSubtaskExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var subtaskId = Guid.NewGuid();
        var command = new UpdateProjectSubtaskCommand(subtaskId, Guid.NewGuid(), "U", true, 2, DateTime.UtcNow);
        var response = new UpdateProjectSubtaskResponse(
            command.Id,
            command.TaskId,
            command.Title,
            command.IsCompleted,
            command.Order,
            command.CompletedAt,
            Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectSubtaskCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, subtaskId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSubtaskDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "U", true, 2, DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectSubtaskCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectSubtaskResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectSubtaskCommand>(), It.IsAny<CancellationToken>()))
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
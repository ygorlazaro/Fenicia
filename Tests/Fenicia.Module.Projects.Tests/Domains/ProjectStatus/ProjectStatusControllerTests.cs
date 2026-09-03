using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class ProjectStatusControllerTests
{
    private readonly ProjectStatusController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IProjectStatusService> _mockService;
    private readonly Guid _testUserId;

    public ProjectStatusControllerTests()
    {
        _mockService = new Mock<IProjectStatusService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectStatusController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenStatusesExist_ReturnsOkWithStatuses()
    {
        var wide = new WideEventContext();
        var statuses = new List<GetAllProjectStatusResponse>
        {
            new(
                Guid.NewGuid(),
                Guid.NewGuid(),
                _faker.Commerce.Categories(1).First(),
                "#FF0000",
                1,
                false,
                Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statuses);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((List<GetAllProjectStatusResponse>)okResult.Value!).Should().HaveCount(1);
    }

    [Fact]
    public async Task GetAsync_WhenNoStatusesExist_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectStatusQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((List<GetAllProjectStatusResponse>)okResult.Value!).Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusExists_ReturnsOkWithStatus()
    {
        var wide = new WideEventContext();
        var status = new GetProjectStatusByIdResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Active",
            "#FF0000",
            1,
            false,
            Guid.NewGuid());

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var result = await _controller.GetByIdAsync(status.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((GetProjectStatusByIdResponse)okResult.Value!).Name.Should().Be("Active");
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectStatusByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectStatusByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "Active", "#FF0000", 1, false);
        var response = new AddProjectStatusResponse(
            command.Id,
            command.ProjectId,
            command.Name,
            command.Color,
            command.Order,
            command.IsFinal,
            Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result!;
        ((AddProjectStatusResponse)createdResult.Value!).Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenStatusExists_ReturnsOkWithUpdatedStatus()
    {
        var wide = new WideEventContext();
        var statusId = Guid.NewGuid();
        var command = new UpdateProjectStatusCommand(statusId, Guid.NewGuid(), "Done", "#00FF00", 2, true);
        var response = new UpdateProjectStatusResponse(
            command.Id,
            command.ProjectId,
            command.Name,
            command.Color,
            command.Order,
            command.IsFinal,
            Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectStatusCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, statusId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((UpdateProjectStatusResponse)okResult.Value!).Name.Should().Be("Done");
    }

    [Fact]
    public async Task PatchAsync_WhenStatusDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), "Done", "#00FF00", 2, true);

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectStatusCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectStatusResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectStatusCommand>(), It.IsAny<CancellationToken>()))
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
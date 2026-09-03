using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Enums.Project;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectControllerTests
{
    private readonly ProjectController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IProjectService> _mockService;
    private readonly Guid _testUserId;

    public ProjectControllerTests()
    {
        _mockService = new Mock<IProjectService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenProjectsExist_ReturnsOkWithProjects()
    {
        var wide = new WideEventContext();
        var projects = new List<GetAllProjectResponse>
        {
            new(
                Guid.NewGuid(),
                _faker.Commerce.Categories(1).First(),
                _faker.Commerce.ProductDescription(),
                nameof(EnumProjectStatus.Active),
                DateTime.UtcNow,
                DateTime.UtcNow.AddMonths(1),
                _testUserId,
                Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        var returned = (List<GetAllProjectResponse>)okResult.Value!;
        returned.Should().HaveCount(1);
        returned.First().Id.Should().Be(projects[0].Id);
        wide.UserId.Should().Be(_testUserId.ToString());
    }

    [Fact]
    public async Task GetAsync_WhenNoProjectsExist_ReturnsOkWithEmptyList()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((List<GetAllProjectResponse>)okResult.Value!).Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsOkWithProject()
    {
        var wide = new WideEventContext();
        var project = new GetProjectByIdResponse(
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Active),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            _testUserId,
            Guid.NewGuid(),
            [],
            []);

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        var result = await _controller.GetByIdAsync(project.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((GetProjectByIdResponse)okResult.Value!).Id.Should().Be(project.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Draft),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            _testUserId);
        var response = new AddProjectResponse(
            command.Id,
            command.Title,
            command.Description,
            nameof(EnumProjectStatus.Draft),
            command.StartDate,
            command.EndDate,
            _testUserId,
            Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result!;
        ((AddProjectResponse)createdResult.Value!).Id.Should().Be(command.Id);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectExists_ReturnsOkWithUpdatedProject()
    {
        var wide = new WideEventContext();
        var projectId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            projectId,
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Active),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            _testUserId);
        var response = new UpdateProjectResponse(
            command.Id,
            command.Title,
            command.Description,
            command.Status,
            command.StartDate,
            command.EndDate,
            command.Owner,
            Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, projectId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result!;
        ((UpdateProjectResponse)okResult.Value!).Id.Should().Be(projectId);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectCommand(
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumProjectStatus.Draft),
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            _testUserId);

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectCommand>(), It.IsAny<CancellationToken>()))
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
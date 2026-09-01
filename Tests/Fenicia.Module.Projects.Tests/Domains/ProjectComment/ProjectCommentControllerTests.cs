using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class ProjectCommentControllerTests
{
    private readonly ProjectCommentController _controller;
    private readonly Faker _faker;
    private readonly Mock<IProjectCommentService> _mockService;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ProjectCommentControllerTests()
    {
        _mockService = new Mock<IProjectCommentService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectCommentController(_mockService.Object) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenCommentsExist_ReturnsOkWithComments()
    {
        var wide = new WideEventContext();
        var comments = new List<GetAllProjectCommentResponse>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence(), Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectCommentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comments);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var comment = new GetProjectCommentByIdResponse(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "hello", Guid.NewGuid());

        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectCommentByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var result = await _controller.GetByIdAsync(comment.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<GetProjectCommentByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectCommentByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), _testUserId, "hello");
        var response = new AddProjectCommentResponse(command.Id, command.TaskId, command.UserId, command.Content, Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCommentExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var commentId = Guid.NewGuid();
        var command = new UpdateProjectCommentCommand(commentId, "updated");
        var response = new UpdateProjectCommentResponse(command.Id, Guid.NewGuid(), _testUserId, command.Content, Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectCommentCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, commentId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), "x");

        _mockService.Setup(s => s.UpdateAsync(It.IsAny<UpdateProjectCommentCommand>(), _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectCommentResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectCommentCommand>(), It.IsAny<CancellationToken>()))
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
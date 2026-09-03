using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

public class ProjectAttachmentControllerTests
{
    private readonly ProjectAttachmentController _controller;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<IProjectAttachmentService> _mockService;
    private readonly Guid _testUserId;

    public ProjectAttachmentControllerTests()
    {
        _mockService = new Mock<IProjectAttachmentService>();
        _mockHttpContext = new Mock<HttpContext>();
        _testUserId = Guid.NewGuid();
        _controller = new ProjectAttachmentController(_mockService.Object)
            { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    [Fact]
    public async Task GetAsync_WhenAttachmentsExist_ReturnsOkWithAttachments()
    {
        var wide = new WideEventContext();
        var attachments = new List<GetAllProjectAttachmentResponse>
        {
            new(
                Guid.NewGuid(),
                Guid.NewGuid(),
                _faker.System.FileName(),
                _faker.Internet.Url(),
                100,
                Guid.NewGuid(),
                Guid.NewGuid())
        };

        _mockService.Setup(s => s.GetAllAsync(It.IsAny<GetAllProjectAttachmentQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attachments);

        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var attachment = new GetProjectAttachmentByIdResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "x.pdf",
            "http://x",
            100,
            Guid.NewGuid(),
            Guid.NewGuid());

        _mockService
            .Setup(s => s.GetByIdAsync(It.IsAny<GetProjectAttachmentByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attachment);

        var result = await _controller.GetByIdAsync(attachment.Id, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        _mockService
            .Setup(s => s.GetByIdAsync(It.IsAny<GetProjectAttachmentByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProjectAttachmentByIdResponse?)null);

        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        var wide = new WideEventContext();
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "x.pdf",
            "http://x",
            100,
            Guid.NewGuid(),
            "application/pdf");
        var response = new AddProjectAttachmentResponse(
            command.Id,
            command.TaskId,
            command.FileName,
            command.FileUrl,
            command.FileSize,
            command.UploadedBy,
            Guid.NewGuid());

        _mockService.Setup(s => s.AddAsync(command, _testUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAttachmentExists_ReturnsOk()
    {
        var wide = new WideEventContext();
        var attachmentId = Guid.NewGuid();
        var command = new UpdateProjectAttachmentCommand(
            attachmentId,
            Guid.NewGuid(),
            "y.pdf",
            "http://y",
            200,
            Guid.NewGuid());
        var response = new UpdateProjectAttachmentResponse(
            command.Id,
            command.TaskId,
            command.FileName,
            command.FileUrl,
            command.FileSize,
            command.UploadedBy,
            Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectAttachmentCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.PatchAsync(command, attachmentId, wide, CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        var wide = new WideEventContext();
        var command = new UpdateProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "y.pdf",
            "http://y",
            200,
            Guid.NewGuid());

        _mockService.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateProjectAttachmentCommand>(),
                _testUserId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((UpdateProjectAttachmentResponse?)null);

        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_ReturnsNoContent()
    {
        var wide = new WideEventContext();
        var id = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(It.IsAny<DeleteProjectAttachmentCommand>(), It.IsAny<CancellationToken>()))
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
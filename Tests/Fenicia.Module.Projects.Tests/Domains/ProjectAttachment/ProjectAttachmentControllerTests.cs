using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

public class ProjectAttachmentControllerTests : IDisposable
{
    private readonly ProjectAttachmentController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public ProjectAttachmentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectAttachmentRepository(_db);
        var service = new ProjectAttachmentService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectAttachmentController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _testUserId = Guid.NewGuid();
        _companyId = companyContext.CompanyId;
        SetupUserClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenAttachmentsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(attachment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_CreatesAttachment()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid(), "application/pdf");

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAttachmentExists_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);
        var command = new UpdateProjectAttachmentCommand(attachment.Id, attachment.TaskId, _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid());

        // Act
        var result = await _controller.PatchAsync(command, attachment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenAttachmentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(attachment.Id, wide, CancellationToken.None);

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

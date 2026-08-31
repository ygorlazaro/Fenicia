using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Attachment;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Attachment;

public class AttachmentControllerTests : IDisposable
{
    private static readonly string[] _fileTypes = ["jpg", "png", "pdf", "docx"];
    private readonly AttachmentController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public AttachmentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new AttachmentRepository(_db);
        var service = new AttachmentService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new AttachmentController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddAttachmentCommand(Guid.NewGuid(), _faker.Internet.Url(), _faker.Random.ArrayElement(_fileTypes), _faker.Random.Long(1, 1000), Guid.NewGuid());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedAttachment = (AddAttachmentResponse)createdResult.Value!;
        returnedAttachment.Id.Should().Be(command.Id);
        returnedAttachment.Url.Should().Be(command.Url);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(attachment.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task GetByCommentAsync_WhenAttachmentsExist_ReturnsOkWithAttachments()
    {
        // Arrange
        var wide = new WideEventContext();
        var commentId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = commentId,
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByCommentAsync(commentId, wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var attachments = (List<GetAttachmentResponse>)okResult.Value!;
        attachments.Should().HaveCount(1);
        attachments.First().Id.Should().Be(attachment.Id);
    }

    [Fact]
    public async Task GetByCommentAsync_WhenNoAttachmentsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();
        var commentId = Guid.NewGuid();

        // Act
        var result = await _controller.GetByCommentAsync(commentId, wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var attachments = (List<GetAttachmentResponse>)okResult.Value!;
        attachments.Should().BeEmpty();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

using System.Security.Claims;
using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class ProjectCommentControllerTests : IDisposable
{
    private readonly ProjectCommentController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public ProjectCommentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectCommentRepository(_db);
        var service = new ProjectCommentService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectCommentController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task GetAsync_WhenCommentsExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = _testUserId,
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = _testUserId,
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(comment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_CreatesComment()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), _testUserId, _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCommentExists_UpdatesComment()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = _testUserId,
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);
        var command = new UpdateProjectCommentCommand(comment.Id, _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, comment.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenCommentDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentExists_DeletesComment()
    {
        // Arrange
        var wide = new WideEventContext();
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = _testUserId,
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(comment.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var deletedComment = await _db.ProjectComments.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == comment.Id);
        deletedComment.Should().NotBeNull();
        deletedComment!.Deleted.Should().NotBeNull();
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

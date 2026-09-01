using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class ProjectSubtaskControllerTests : IDisposable
{
    private readonly ProjectSubtaskController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _companyId;

    public ProjectSubtaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectSubtaskRepository(_db);
        var service = new ProjectSubtaskService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectSubtaskController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        var testUserId = Guid.NewGuid();
        _companyId = companyContext.CompanyId;
        SetupUserClaims(testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenSubtasksExist_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(subtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskExists_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(subtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(subtask.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenSubtaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_CreatesSubtask()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence(), false, 1, null);

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSubtaskExists_ReturnsOk()
    {
        // Arrange
        var wide = new WideEventContext();
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(subtask);
        await _db.SaveChangesAsync(CancellationToken.None);
        var command = new UpdateProjectSubtaskCommand(subtask.Id, Guid.NewGuid(), "Updated Title", true, 2, DateTime.UtcNow);

        // Act
        var result = await _controller.PatchAsync(command, subtask.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PatchAsync_WhenSubtaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), "Updated Title", true, 2, DateTime.UtcNow);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenSubtaskExists_DeletesSubtask()
    {
        // Arrange
        var wide = new WideEventContext();
        var subtask = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = _faker.Lorem.Sentence(),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null,
            CompanyId = _companyId
        };
        _db.ProjectSubtasks.Add(subtask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(subtask.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var deletedSubtask = await _db.ProjectSubtasks.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == subtask.Id);
        deletedSubtask.Should().NotBeNull();
        deletedSubtask.Deleted.Should().NotBeNull();
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

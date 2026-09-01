using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskControllerTests : IDisposable
{
    private readonly ProjectTaskController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public ProjectTaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectTaskRepository(_db);
        var service = new ProjectTaskService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectTaskController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task GetAsync_WhenTasksExist_ReturnsOkWithTasks()
    {
        // Arrange
        var wide = new WideEventContext();
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = _faker.Random.Int(1, 100),
            EstimatePoints = _faker.Random.Int(1, 21),
            DueDate = DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            CreatedBy = _testUserId,
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var tasks = (List<GetAllProjectTaskResponse>)okResult.Value!;
        tasks.Should().HaveCount(1);
        tasks.First().Id.Should().Be(projectTask.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsOkWithTask()
    {
        // Arrange
        var wide = new WideEventContext();
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = _faker.Random.Int(1, 100),
            EstimatePoints = _faker.Random.Int(1, 21),
            DueDate = DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            CreatedBy = _testUserId,
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(projectTask.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedTask = (GetProjectTaskByIdResponse)okResult.Value!;
        returnedTask.Id.Should().Be(projectTask.Id);
        returnedTask.Title.Should().Be(projectTask.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumTaskPriority.Medium),
            nameof(EnumTaskType.Task),
            _faker.Random.Int(1, 100),
            _faker.Random.Int(1, 21),
            DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            _testUserId);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedTask = (AddProjectTaskResponse)createdResult.Value!;
        returnedTask.Id.Should().Be(command.Id);
        returnedTask.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenTaskExists_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var wide = new WideEventContext();
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = _faker.Random.Int(1, 100),
            EstimatePoints = _faker.Random.Int(1, 21),
            DueDate = DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            CreatedBy = _testUserId,
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(
            projectTask.Id,
            projectTask.ProjectId,
            projectTask.StatusId,
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumTaskPriority.High),
            nameof(EnumTaskType.Bug),
            _faker.Random.Int(1, 100),
            _faker.Random.Int(1, 21),
            DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            _testUserId);

        // Act
        var result = await _controller.PatchAsync(command, projectTask.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedTask = (UpdateProjectTaskResponse)okResult.Value!;
        returnedTask.Id.Should().Be(projectTask.Id);
        returnedTask.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            _faker.Commerce.Categories(1).First(),
            _faker.Commerce.ProductDescription(),
            nameof(EnumTaskPriority.Medium),
            nameof(EnumTaskType.Task),
            _faker.Random.Int(1, 100),
            _faker.Random.Int(1, 21),
            DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            _testUserId);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var projectTask = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = _faker.Random.Int(1, 100),
            EstimatePoints = _faker.Random.Int(1, 21),
            DueDate = DateTime.UtcNow.AddDays(_faker.Random.Int(1, 30)),
            CreatedBy = _testUserId,
            CompanyId = _companyId
        };
        _db.ProjectTasks.Add(projectTask);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(projectTask.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        var deletedTask = await _db.ProjectTasks.IgnoreQueryFilters().FirstOrDefaultAsync(t => t.Id == projectTask.Id);
        deletedTask.Should().NotBeNull();
        deletedTask.Deleted.Should().NotBeNull();
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

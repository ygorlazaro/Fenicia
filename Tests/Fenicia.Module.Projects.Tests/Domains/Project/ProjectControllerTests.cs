using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectControllerTests : IDisposable
{
    private readonly ProjectController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;
    private readonly Guid _companyId;

    public ProjectControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectRepository(_db);
        var service = new ProjectService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task GetAsync_WhenProjectsExist_ReturnsOkWithProjects()
    {
        // Arrange
        var wide = new WideEventContext();
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = _testUserId,
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var projects = (List<GetAllProjectResponse>)okResult.Value!;
        projects.Should().HaveCount(1);
        projects.First().Id.Should().Be(project.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNoProjectsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var projects = (List<GetAllProjectResponse>)okResult.Value!;
        projects.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsOkWithProject()
    {
        // Arrange
        var wide = new WideEventContext();
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = _testUserId,
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(project.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedProject = (GetProjectByIdResponse)okResult.Value!;
        returnedProject.Id.Should().Be(project.Id);
        returnedProject.Title.Should().Be(project.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFound()
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
        var command = new AddProjectCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First(), _faker.Commerce.ProductDescription(), EnumProjectStatus.Draft.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), _testUserId);
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedProject = (AddProjectResponse)createdResult.Value!;
        returnedProject.Id.Should().Be(command.Id);
        returnedProject.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectExists_ReturnsOkWithUpdatedProject()
    {
        // Arrange
        var wide = new WideEventContext();
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = _testUserId,
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(project.Id, _faker.Commerce.Categories(1).First(), _faker.Commerce.ProductDescription(), EnumProjectStatus.Active.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), _testUserId);

        // Act
        var result = await _controller.PatchAsync(command, project.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedProject = (UpdateProjectResponse)okResult.Value!;
        returnedProject.Id.Should().Be(project.Id);
        returnedProject.Title.Should().Be(command.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectCommand(Guid.NewGuid(), _faker.Commerce.Categories(1).First(), _faker.Commerce.ProductDescription(), EnumProjectStatus.Draft.ToString(), DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), _testUserId);

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var project = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = _faker.Commerce.Categories(1).First(),
            Description = _faker.Commerce.ProductDescription(),
            Status = EnumProjectStatus.Active,
            Owner = _testUserId,
            CompanyId = _companyId
        };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(project.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.DeleteAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
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
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

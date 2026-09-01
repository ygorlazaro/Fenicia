using System.Security.Claims;

using AwesomeAssertions;
using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class ProjectStatusControllerTests : IDisposable
{
    private readonly ProjectStatusController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _companyId;

    public ProjectStatusControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectStatusRepository(_db);
        var service = new ProjectStatusService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ProjectStatusController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
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
    public async Task GetAsync_WhenStatusesExist_ReturnsOkWithStatuses()
    {
        // Arrange
        var wide = new WideEventContext();
        var status = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = _faker.Commerce.Categories(1).First(),
            Color = _faker.Commerce.Color(),
            Order = _faker.Random.Int(1, 10),
            IsFinal = _faker.Random.Bool(),
            CompanyId = _companyId
        };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var statuses = (List<GetAllProjectStatusResponse>)okResult.Value!;
        statuses.Should().HaveCount(1);
        statuses.First().Id.Should().Be(status.Id);
    }

    [Fact]
    public async Task GetAsync_WhenNoStatusesExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAsync(wide, 1, 10, null, null, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var statuses = (List<GetAllProjectStatusResponse>)okResult.Value!;
        statuses.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusExists_ReturnsOkWithStatus()
    {
        // Arrange
        var wide = new WideEventContext();
        var status = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = _faker.Commerce.Categories(1).First(),
            Color = _faker.Commerce.Color(),
            Order = _faker.Random.Int(1, 10),
            IsFinal = _faker.Random.Bool(),
            CompanyId = _companyId
        };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.GetByIdAsync(status.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedStatus = (GetProjectStatusByIdResponse)okResult.Value!;
        returnedStatus.Id.Should().Be(status.Id);
        returnedStatus.Name.Should().Be(status.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusDoesNotExist_ReturnsNotFound()
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
        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Commerce.Categories(1).First(), _faker.Commerce.Color(), _faker.Random.Int(1, 10), _faker.Random.Bool());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedStatus = (AddProjectStatusResponse)createdResult.Value!;
        returnedStatus.Id.Should().Be(command.Id);
        returnedStatus.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenStatusExists_ReturnsOkWithUpdatedStatus()
    {
        // Arrange
        var wide = new WideEventContext();
        var status = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = _faker.Commerce.Categories(1).First(),
            Color = _faker.Commerce.Color(),
            Order = _faker.Random.Int(1, 10),
            IsFinal = _faker.Random.Bool(),
            CompanyId = _companyId
        };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(status.Id, status.ProjectId, _faker.Commerce.Categories(1).First(), _faker.Commerce.Color(), _faker.Random.Int(1, 10), _faker.Random.Bool());

        // Act
        var result = await _controller.PatchAsync(command, status.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedStatus = (UpdateProjectStatusResponse)okResult.Value!;
        returnedStatus.Id.Should().Be(status.Id);
        returnedStatus.Name.Should().Be(command.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenStatusDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Commerce.Categories(1).First(), _faker.Commerce.Color(), _faker.Random.Int(1, 10), _faker.Random.Bool());

        // Act
        var result = await _controller.PatchAsync(command, Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenStatusExists_ReturnsNoContent()
    {
        // Arrange
        var wide = new WideEventContext();
        var status = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = _faker.Commerce.Categories(1).First(),
            Color = _faker.Commerce.Color(),
            Order = _faker.Random.Int(1, 10),
            IsFinal = _faker.Random.Bool(),
            CompanyId = _companyId
        };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _controller.DeleteAsync(status.Id, wide, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteAsync_WhenStatusDoesNotExist_ReturnsNoContent()
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

using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.Add;
using Fenicia.Module.Projects.Domains.Project.Delete;
using Fenicia.Module.Projects.Domains.Project.GetAll;
using Fenicia.Module.Projects.Domains.Project.GetById;
using Fenicia.Module.Projects.Domains.Project.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

public class ProjectControllerTests : IDisposable
{
    private readonly ProjectController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectId;

    public ProjectControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.testProjectId = Guid.NewGuid();
        var getAllProjectHandler = new GetAllProjectHandler(this.db);
        var getProjectByIdHandler = new GetProjectByIdHandler(this.db);
        var addProjectHandler = new AddProjectHandler(this.db);
        var updateProjectHandler = new UpdateProjectHandler(this.db);
        var deleteProjectHandler = new DeleteProjectHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectController(getAllProjectHandler, getProjectByIdHandler, addProjectHandler, updateProjectHandler, deleteProjectHandler) { ControllerContext = new ControllerContext { HttpContext = this.mockHttpContext.Object } };

        SetupUserClaims();
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenNoProjectsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProjects = okResult.Value as List<GetAllProjectResponse>;
        Assert.NotNull(returnedProjects);
        Assert.Empty(returnedProjects);
    }

    [Fact]
    public async Task GetAsync_WhenProjectsExist_ReturnsOkWithProjects()
    {
        // Arrange
        var project1 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.AddRange(project1, project2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProjects = okResult.Value as List<GetAllProjectResponse>;
        Assert.NotNull(returnedProjects);
        Assert.Equal(2, returnedProjects.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsOkWithProject()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProjectId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProject = okResult.Value as GetProjectByIdResponse;
        Assert.NotNull(returnedProject);
        Assert.Equal(this.testProjectId, returnedProject.Id);
        Assert.Equal(project.Title, returnedProject.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithProject()
    {
        // Arrange
        var command = new AddProjectCommand(Guid.NewGuid(), this.faker.Lorem.Sentence(5), this.faker.Lorem.Paragraph(), "Active", DateTime.UtcNow, null, Guid.NewGuid());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedProject = createdResult.Value as AddProjectResponse;
        Assert.NotNull(returnedProject);
        Assert.Equal(command.Id, returnedProject.Id);
        Assert.Equal(command.Title, returnedProject.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectExists_ReturnsOkWithUpdatedProject()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(project.Id, this.faker.Lorem.Sentence(5) + " Updated", this.faker.Lorem.Paragraph(), "Completed", project.StartDate, DateTime.UtcNow, project.Owner);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testProjectId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProject = okResult.Value as UpdateProjectResponse;
        Assert.NotNull(returnedProject);
        Assert.Contains("Updated", returnedProject.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommand(nonExistentId, this.faker.Lorem.Sentence(5), this.faker.Lorem.Paragraph(), "Active", DateTime.UtcNow, null, Guid.NewGuid());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_ReturnsNoContent()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.db.Projects.Add(project);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProjectId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify project was deleted
        var deletedProject = await this.db.Projects.FirstOrDefaultAsync(x => x.Id == this.testProjectId && x.Deleted == null, ct);
        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void ProjectController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ProjectController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ProjectController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void ProjectController_DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void ProjectController_PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var postMethod = controllerType.GetMethod(nameof(ProjectController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void ProjectController_PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}
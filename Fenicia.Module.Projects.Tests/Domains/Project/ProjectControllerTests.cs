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
        db = new DefaultContext(options, companyContext);
        testProjectId = Guid.NewGuid();
        var getAllProjectHandler = new GetAllProjectHandler(db);
        var getProjectByIdHandler = new GetProjectByIdHandler(db);
        var addProjectHandler = new AddProjectHandler(db);
        var updateProjectHandler = new UpdateProjectHandler(db);
        var deleteProjectHandler = new DeleteProjectHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectController(getAllProjectHandler, getProjectByIdHandler, addProjectHandler, updateProjectHandler, deleteProjectHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenNoProjectsExist_ReturnsOkWithEmptyList()
    {

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

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

        var project1 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        var project2 = new ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.AddRange(project1, project2);
        await db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

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

        var project = new ProjectModel
        {
            Id = testProjectId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedProject = okResult.Value as GetProjectByIdResponse;
        Assert.NotNull(returnedProject);
        Assert.Equal(testProjectId, returnedProject.Id);
        Assert.Equal(project.Title, returnedProject.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithProject()
    {

        var command = new AddProjectCommand(Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Active", DateTime.UtcNow, null, Guid.NewGuid());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

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

        var project = new ProjectModel
        {
            Id = testProjectId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(project.Id, faker.Lorem.Sentence(5) + " Updated", faker.Lorem.Paragraph(), "Completed", project.StartDate, DateTime.UtcNow, project.Owner);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectId, wide, ct);

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

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommand(nonExistentId, faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Active", DateTime.UtcNow, null, Guid.NewGuid());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectExists_ReturnsNoContent()
    {

        var project = new ProjectModel
        {
            Id = testProjectId,
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Status = EnumProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectId, wide, ct);

        Assert.NotNull(result);

        var deletedProject = await db.Projects.FirstOrDefaultAsync(x => x.Id == testProjectId && x.Deleted == null, ct);
        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ReturnsNoContent()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void ProjectController_HasAuthorizeAttribute()
    {

        var controllerType = typeof(ProjectController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void ProjectController_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void ProjectController_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void ProjectController_DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void ProjectController_PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectController);
        var postMethod = controllerType.GetMethod(nameof(ProjectController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void ProjectController_PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

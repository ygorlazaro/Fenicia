using System.Security.Claims;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Delete;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTaskAssignee;

public class ProjectTaskAssigneeControllerTests : IDisposable
{
    private readonly ProjectTaskAssigneeController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectTaskAssigneeId;

    public ProjectTaskAssigneeControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectTaskAssigneeId = Guid.NewGuid();
        var getAllProjectTaskAssigneeHandler = new GetAllProjectTaskAssigneeHandler(db);
        var getProjectTaskAssigneeByIdHandler = new GetProjectTaskAssigneeByIdHandler(db);
        var addProjectTaskAssigneeHandler = new AddProjectTaskAssigneeHandler(db);
        var updateProjectTaskAssigneeHandler = new UpdateProjectTaskAssigneeHandler(db);
        var deleteProjectTaskAssigneeHandler = new DeleteProjectTaskAssigneeHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectTaskAssigneeController(getAllProjectTaskAssigneeHandler, getProjectTaskAssigneeByIdHandler, addProjectTaskAssigneeHandler, updateProjectTaskAssigneeHandler, deleteProjectTaskAssigneeHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
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
    public async Task GetAsync_WhenNoItemsExist_ReturnsOkWithEmptyList()
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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.NotNull(returnedAssignees);
        Assert.Empty(returnedAssignees);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {

        var projectTaskAssignee1 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        var projectTaskAssignee2 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-1)
        };

        db.ProjectTaskAssignees.AddRange(projectTaskAssignee1, projectTaskAssignee2);
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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.NotNull(returnedAssignees);
        Assert.Equal(2, returnedAssignees.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        db.ProjectTaskAssignees.Add(projectTaskAssignee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectTaskAssigneeId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAssignee = okResult.Value as GetProjectTaskAssigneeByIdResponse;
        Assert.NotNull(returnedAssignee);
        Assert.Equal(testProjectTaskAssigneeId, returnedAssignee.Id);
        Assert.Equal(projectTaskAssignee.TaskId, returnedAssignee.TaskId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithItem()
    {

        var command = new AddProjectTaskAssigneeCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedAssignee = createdResult.Value as AddProjectTaskAssigneeResponse;
        Assert.NotNull(returnedAssignee);
        Assert.Equal(command.Id, returnedAssignee.Id);
        Assert.Equal(command.TaskId, returnedAssignee.TaskId);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        db.ProjectTaskAssignees.Add(projectTaskAssignee);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskAssigneeCommand(projectTaskAssignee.Id, projectTaskAssignee.TaskId, projectTaskAssignee.UserId, "Contributor", projectTaskAssignee.AssignedAt);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectTaskAssigneeId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAssignee = okResult.Value as UpdateProjectTaskAssigneeResponse;
        Assert.NotNull(returnedAssignee);
        Assert.Equal("Contributor", returnedAssignee.Role);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectTaskAssigneeCommand(nonExistentId, Guid.NewGuid(), Guid.NewGuid(), "Owner", DateTime.UtcNow);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        db.ProjectTaskAssignees.Add(projectTaskAssignee);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectTaskAssigneeId, wide, ct);

        Assert.NotNull(result);

        var deletedAssignee = await db.ProjectTaskAssignees.FirstOrDefaultAsync(x => x.Id == testProjectTaskAssigneeId && x.Deleted == null, ct);
        Assert.Null(deletedAssignee);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void Controller_HasAuthorizeAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskAssigneeController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

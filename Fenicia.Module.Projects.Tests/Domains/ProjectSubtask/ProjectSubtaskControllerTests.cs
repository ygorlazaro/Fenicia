using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Add;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Delete;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;
using Fenicia.Module.Projects.Domains.ProjectSubtask.GetById;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectSubtask;

public class ProjectSubtaskControllerTests : IDisposable
{
    private readonly ProjectSubtaskController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectSubtaskId;

    public ProjectSubtaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectSubtaskId = Guid.NewGuid();
        var getAllProjectSubtaskHandler = new GetAllProjectSubtaskHandler(db);
        var getProjectSubtaskByIdHandler = new GetProjectSubtaskByIdHandler(db);
        var addProjectSubtaskHandler = new AddProjectSubtaskHandler(db);
        var updateProjectSubtaskHandler = new UpdateProjectSubtaskHandler(db);
        var deleteProjectSubtaskHandler = new DeleteProjectSubtaskHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectSubtaskController(getAllProjectSubtaskHandler, getProjectSubtaskByIdHandler, addProjectSubtaskHandler, updateProjectSubtaskHandler, deleteProjectSubtaskHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

        var returnedSubtasks = okResult.Value as List<GetAllProjectSubtaskResponse>;
        Assert.NotNull(returnedSubtasks);
        Assert.Empty(returnedSubtasks);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {

        var projectSubtask1 = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var projectSubtask2 = new ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow
        };

        db.ProjectSubtasks.AddRange(projectSubtask1, projectSubtask2);
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

        var returnedSubtasks = okResult.Value as List<GetAllProjectSubtaskResponse>;
        Assert.NotNull(returnedSubtasks);
        Assert.Equal(2, returnedSubtasks.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectSubtask = new ProjectSubtaskModel
        {
            Id = testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        db.ProjectSubtasks.Add(projectSubtask);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectSubtaskId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSubtask = okResult.Value as GetProjectSubtaskByIdResponse;
        Assert.NotNull(returnedSubtask);
        Assert.Equal(testProjectSubtaskId, returnedSubtask.Id);
        Assert.Equal(projectSubtask.Title, returnedSubtask.Title);
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

        var command = new AddProjectSubtaskCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.PickRandom(true, false), faker.Random.Int(1, 10), faker.PickRandom<DateTime?>(null, DateTime.UtcNow));

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedSubtask = createdResult.Value as AddProjectSubtaskResponse;
        Assert.NotNull(returnedSubtask);
        Assert.Equal(command.Id, returnedSubtask.Id);
        Assert.Equal(command.Title, returnedSubtask.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectSubtask = new ProjectSubtaskModel
        {
            Id = testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        db.ProjectSubtasks.Add(projectSubtask);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(projectSubtask.Id, projectSubtask.TaskId, faker.Lorem.Sentence(5) + " Updated", true, projectSubtask.Order, DateTime.UtcNow);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectSubtaskId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedSubtask = okResult.Value as UpdateProjectSubtaskResponse;
        Assert.NotNull(returnedSubtask);
        Assert.Contains("Updated", returnedSubtask.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectSubtaskCommand(nonExistentId, Guid.NewGuid(), faker.Lorem.Sentence(5), faker.PickRandom(true, false), faker.Random.Int(1, 10), faker.PickRandom<DateTime?>(null, DateTime.UtcNow));

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectSubtask = new ProjectSubtaskModel
        {
            Id = testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        db.ProjectSubtasks.Add(projectSubtask);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectSubtaskId, wide, ct);

        Assert.NotNull(result);

        var deletedSubtask = await db.ProjectSubtasks.FirstOrDefaultAsync(x => x.Id == testProjectSubtaskId && x.Deleted == null, ct);
        Assert.Null(deletedSubtask);
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

        var controllerType = typeof(ProjectSubtaskController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectSubtaskController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectSubtaskController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectSubtaskController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectSubtaskController);
        var postMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectSubtaskController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

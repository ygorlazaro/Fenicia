using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.Add;
using Fenicia.Module.Projects.Domains.ProjectStatus.Delete;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetAll;
using Fenicia.Module.Projects.Domains.ProjectStatus.GetById;
using Fenicia.Module.Projects.Domains.ProjectStatus.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class ProjectStatusControllerTests : IDisposable
{
    private readonly ProjectStatusController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectStatusId;

    public ProjectStatusControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectStatusId = Guid.NewGuid();
        var getAllProjectStatusHandler = new GetAllProjectStatusHandler(db);
        var getProjectStatusByIdHandler = new GetProjectStatusByIdHandler(db);
        var addProjectStatusHandler = new AddProjectStatusHandler(db);
        var updateProjectStatusHandler = new UpdateProjectStatusHandler(db);
        var deleteProjectStatusHandler = new DeleteProjectStatusHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectStatusController(getAllProjectStatusHandler, getProjectStatusByIdHandler, addProjectStatusHandler, updateProjectStatusHandler, deleteProjectStatusHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

        var returnedStatuses = okResult.Value as List<GetAllProjectStatusResponse>;
        Assert.NotNull(returnedStatuses);
        Assert.Empty(returnedStatuses);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {

        var projectStatus1 = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var projectStatus2 = new ProjectStatusModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        db.ProjectStatuses.AddRange(projectStatus1, projectStatus2);
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

        var returnedStatuses = okResult.Value as List<GetAllProjectStatusResponse>;
        Assert.NotNull(returnedStatuses);
        Assert.Equal(2, returnedStatuses.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectStatus = new ProjectStatusModel
        {
            Id = testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        db.ProjectStatuses.Add(projectStatus);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectStatusId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedStatus = okResult.Value as GetProjectStatusByIdResponse;
        Assert.NotNull(returnedStatus);
        Assert.Equal(testProjectStatusId, returnedStatus.Id);
        Assert.Equal(projectStatus.Name, returnedStatus.Name);
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

        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), faker.Random.Int(1, 10), faker.PickRandom(true, false));

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedStatus = createdResult.Value as AddProjectStatusResponse;
        Assert.NotNull(returnedStatus);
        Assert.Equal(command.Id, returnedStatus.Id);
        Assert.Equal(command.Name, returnedStatus.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectStatus = new ProjectStatusModel
        {
            Id = testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        db.ProjectStatuses.Add(projectStatus);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(projectStatus.Id, projectStatus.ProjectId, faker.Lorem.Word() + " Updated", faker.Internet.Color(), projectStatus.Order, projectStatus.IsFinal);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectStatusId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedStatus = okResult.Value as UpdateProjectStatusResponse;
        Assert.NotNull(returnedStatus);
        Assert.Contains("Updated", returnedStatus.Name);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectStatusCommand(nonExistentId, Guid.NewGuid(), faker.Lorem.Word(), faker.Internet.Color(), faker.Random.Int(1, 10), faker.PickRandom(true, false));

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectStatus = new ProjectStatusModel
        {
            Id = testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = faker.Lorem.Word(),
            Color = faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        db.ProjectStatuses.Add(projectStatus);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectStatusId, wide, ct);

        Assert.NotNull(result);

        var deletedStatus = await db.ProjectStatuses.FirstOrDefaultAsync(x => x.Id == testProjectStatusId && x.Deleted == null, ct);
        Assert.Null(deletedStatus);
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

        var controllerType = typeof(ProjectStatusController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectStatusController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectStatusController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectStatusController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectStatusController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectStatusController);
        var postMethod = controllerType.GetMethod(nameof(ProjectStatusController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectStatusController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectStatusController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

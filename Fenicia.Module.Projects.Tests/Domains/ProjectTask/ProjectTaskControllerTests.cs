using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.Add;
using Fenicia.Module.Projects.Domains.ProjectTask.Delete;
using Fenicia.Module.Projects.Domains.ProjectTask.GetAll;
using Fenicia.Module.Projects.Domains.ProjectTask.GetById;
using Fenicia.Module.Projects.Domains.ProjectTask.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskControllerTests : IDisposable
{
    private readonly ProjectTaskController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectTaskId;

    public ProjectTaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectTaskId = Guid.NewGuid();
        var getAllProjectTaskHandler = new GetAllProjectTaskHandler(db);
        var getProjectTaskByIdHandler = new GetProjectTaskByIdHandler(db);
        var addProjectTaskHandler = new AddProjectTaskHandler(db);
        var updateProjectTaskHandler = new UpdateProjectTaskHandler(db);
        var deleteProjectTaskHandler = new DeleteProjectTaskHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectTaskController(getAllProjectTaskHandler, getProjectTaskByIdHandler, addProjectTaskHandler, updateProjectTaskHandler, deleteProjectTaskHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.NotNull(returnedTasks);
        Assert.Empty(returnedTasks);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {

        var projectTask1 = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        var projectTask2 = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.High,
            Type = EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 8,
            DueDate = DateTime.UtcNow.AddDays(14),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.AddRange(projectTask1, projectTask2);
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

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.NotNull(returnedTasks);
        Assert.Equal(2, returnedTasks.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectTask = new ProjectTaskModel
        {
            Id = testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(projectTask);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectTaskId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedTask = okResult.Value as GetProjectTaskByIdResponse;
        Assert.NotNull(returnedTask);
        Assert.Equal(testProjectTaskId, returnedTask.Id);
        Assert.Equal(projectTask.Title, returnedTask.Title);
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

        var command = new AddProjectTaskCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Medium", "Task", faker.Random.Int(1, 10), faker.Random.Int(1, 13), DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedTask = createdResult.Value as AddProjectTaskResponse;
        Assert.NotNull(returnedTask);
        Assert.Equal(command.Id, returnedTask.Id);
        Assert.Equal(command.Title, returnedTask.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectTask = new ProjectTaskModel
        {
            Id = testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(projectTask);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(projectTask.Id, projectTask.ProjectId, projectTask.StatusId, faker.Lorem.Sentence(5) + " Updated", faker.Lorem.Paragraph(), "High", "Bug", projectTask.Order, projectTask.EstimatePoints, projectTask.DueDate, projectTask.CreatedBy);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectTaskId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedTask = okResult.Value as UpdateProjectTaskResponse;
        Assert.NotNull(returnedTask);
        Assert.Contains("Updated", returnedTask.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectTaskCommand(nonExistentId, Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Sentence(5), faker.Lorem.Paragraph(), "Medium", "Task", faker.Random.Int(1, 10), faker.Random.Int(1, 13), DateTime.UtcNow.AddDays(7), Guid.NewGuid());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectTask = new ProjectTaskModel
        {
            Id = testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = faker.Lorem.Sentence(5),
            Description = faker.Lorem.Paragraph(),
            Priority = EnumTaskPriority.Medium,
            Type = EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        db.ProjectTasks.Add(projectTask);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectTaskId, wide, ct);

        Assert.NotNull(result);

        var deletedTask = await db.ProjectTasks.FirstOrDefaultAsync(x => x.Id == testProjectTaskId && x.Deleted == null, ct);
        Assert.Null(deletedTask);
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

        var controllerType = typeof(ProjectTaskController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectTaskController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectTaskController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectTaskController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

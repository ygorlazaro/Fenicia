using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
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
    public ProjectTaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.testProjectTaskId = Guid.NewGuid();
        var getAllProjectTaskHandler = new GetAllProjectTaskHandler(this.db);
        var getProjectTaskByIdHandler = new GetProjectTaskByIdHandler(this.db);
        var addProjectTaskHandler = new AddProjectTaskHandler(this.db);
        var updateProjectTaskHandler = new UpdateProjectTaskHandler(this.db);
        var deleteProjectTaskHandler = new DeleteProjectTaskHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectTaskController(
            getAllProjectTaskHandler,
            getProjectTaskByIdHandler,
            addProjectTaskHandler,
            updateProjectTaskHandler,
            deleteProjectTaskHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly ProjectTaskController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectTaskId;
    private readonly Faker faker;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId", Guid.NewGuid().ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        this.mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        this.controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenNoItemsExist_ReturnsOkWithEmptyList()
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

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.NotNull(returnedTasks);
        Assert.Empty(returnedTasks);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectTask1 = new ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Task,
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
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.High,
            Type = Common.Enums.Project.EnumTaskType.Bug,
            Order = 2,
            EstimatePoints = 8,
            DueDate = DateTime.UtcNow.AddDays(14),
            CreatedBy = Guid.NewGuid()
        };

        this.db.ProjectTasks.AddRange(projectTask1, projectTask2);
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

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.NotNull(returnedTasks);
        Assert.Equal(2, returnedTasks.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.db.ProjectTasks.Add(projectTask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProjectTaskId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedTask = okResult.Value as GetProjectTaskByIdResponse;
        Assert.NotNull(returnedTask);
        Assert.Equal(this.testProjectTaskId, returnedTask.Id);
        Assert.Equal(projectTask.Title, returnedTask.Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
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
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithItem()
    {
        // Arrange
        var command = new AddProjectTaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            this.faker.Random.Int(1, 10),
            this.faker.Random.Int(1, 13),
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

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

        var returnedTask = createdResult.Value as AddProjectTaskResponse;
        Assert.NotNull(returnedTask);
        Assert.Equal(command.Id, returnedTask.Id);
        Assert.Equal(command.Title, returnedTask.Title);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.db.ProjectTasks.Add(projectTask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskCommand(
            projectTask.Id,
            projectTask.ProjectId,
            projectTask.StatusId,
            this.faker.Lorem.Sentence(5) + " Updated",
            this.faker.Lorem.Paragraph(),
            "High",
            "Bug",
            projectTask.Order,
            projectTask.EstimatePoints,
            projectTask.DueDate,
            projectTask.CreatedBy);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testProjectTaskId, wide, ct);

        // Assert
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
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectTaskCommand(
            nonExistentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Medium",
            "Task",
            this.faker.Random.Int(1, 10),
            this.faker.Random.Int(1, 13),
            DateTime.UtcNow.AddDays(7),
            Guid.NewGuid());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var projectTask = new ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.EnumTaskPriority.Medium,
            Type = Common.Enums.Project.EnumTaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.db.ProjectTasks.Add(projectTask);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProjectTaskId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify project task was deleted
        var deletedTask = await this.db.ProjectTasks.FirstOrDefaultAsync(x => x.Id == this.testProjectTaskId && x.Deleted == null, ct);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
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
    public void Controller_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

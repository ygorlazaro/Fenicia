using System.Security.Claims;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
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
    public ProjectTaskAssigneeControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testProjectTaskAssigneeId = Guid.NewGuid();
        var getAllProjectTaskAssigneeHandler = new GetAllProjectTaskAssigneeHandler(this.context);
        var getProjectTaskAssigneeByIdHandler = new GetProjectTaskAssigneeByIdHandler(this.context);
        var addProjectTaskAssigneeHandler = new AddProjectTaskAssigneeHandler(this.context);
        var updateProjectTaskAssigneeHandler = new UpdateProjectTaskAssigneeHandler(this.context);
        var deleteProjectTaskAssigneeHandler = new DeleteProjectTaskAssigneeHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectTaskAssigneeController(
            getAllProjectTaskAssigneeHandler,
            getProjectTaskAssigneeByIdHandler,
            addProjectTaskAssigneeHandler,
            updateProjectTaskAssigneeHandler,
            deleteProjectTaskAssigneeHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly ProjectTaskAssigneeController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectTaskAssigneeId;

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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.NotNull(returnedAssignees);
        Assert.Empty(returnedAssignees);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectTaskAssignee1 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        var projectTaskAssignee2 = new TaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = Common.Enums.Project.EnumAssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-1)
        };

        this.context.ProjectTaskAssignees.AddRange(projectTaskAssignee1, projectTaskAssignee2);
        await this.context.SaveChangesAsync(CancellationToken.None);

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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.NotNull(returnedAssignees);
        Assert.Equal(2, returnedAssignees.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        this.context.ProjectTaskAssignees.Add(projectTaskAssignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProjectTaskAssigneeId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAssignee = okResult.Value as GetProjectTaskAssigneeByIdResponse;
        Assert.NotNull(returnedAssignee);
        Assert.Equal(this.testProjectTaskAssigneeId, returnedAssignee.Id);
        Assert.Equal(projectTaskAssignee.TaskId, returnedAssignee.TaskId);
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
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

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

        var returnedAssignee = createdResult.Value as AddProjectTaskAssigneeResponse;
        Assert.NotNull(returnedAssignee);
        Assert.Equal(command.Id, returnedAssignee.Id);
        Assert.Equal(command.TaskId, returnedAssignee.TaskId);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        this.context.ProjectTaskAssignees.Add(projectTaskAssignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectTaskAssigneeCommand(
            projectTaskAssignee.Id,
            projectTaskAssignee.TaskId,
            projectTaskAssignee.UserId,
            "Contributor",
            projectTaskAssignee.AssignedAt);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testProjectTaskAssigneeId, wide, ct);

        // Assert
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
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectTaskAssigneeCommand(
            nonExistentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

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
        var projectTaskAssignee = new TaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role = Common.Enums.Project.EnumAssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        this.context.ProjectTaskAssignees.Add(projectTaskAssignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProjectTaskAssigneeId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify project task assignee was deleted
        var deletedAssignee = await this.context.ProjectTaskAssignees.FirstOrDefaultAsync(x => x.Id == this.testProjectTaskAssigneeId && x.Deleted == null, ct);
        Assert.Null(deletedAssignee);
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
        var controllerType = typeof(ProjectTaskAssigneeController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskAssigneeController);

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
        var controllerType = typeof(ProjectTaskAssigneeController);

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
        var controllerType = typeof(ProjectTaskAssigneeController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.DeleteAsync));

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
        var controllerType = typeof(ProjectTaskAssigneeController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PostAsync));

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
        var controllerType = typeof(ProjectTaskAssigneeController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

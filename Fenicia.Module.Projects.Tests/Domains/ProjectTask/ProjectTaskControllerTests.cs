using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
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

[TestFixture]
public class ProjectTaskControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectTaskId = Guid.NewGuid();
        this.getAllProjectTaskHandler = new GetAllProjectTaskHandler(this.context);
        this.getProjectTaskByIdHandler = new GetProjectTaskByIdHandler(this.context);
        this.addProjectTaskHandler = new AddProjectTaskHandler(this.context);
        this.updateProjectTaskHandler = new UpdateProjectTaskHandler(this.context);
        this.deleteProjectTaskHandler = new DeleteProjectTaskHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectTaskController(
            this.getAllProjectTaskHandler,
            this.getProjectTaskByIdHandler,
            this.addProjectTaskHandler,
            this.updateProjectTaskHandler,
            this.deleteProjectTaskHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private ProjectTaskController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectTaskHandler getAllProjectTaskHandler = null!;
    private GetProjectTaskByIdHandler getProjectTaskByIdHandler = null!;
    private AddProjectTaskHandler addProjectTaskHandler = null!;
    private UpdateProjectTaskHandler updateProjectTaskHandler = null!;
    private DeleteProjectTaskHandler deleteProjectTaskHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectTaskId;
    private Faker faker = null!;

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

    [Test]
    public async Task GetAsync_WhenNoItemsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.That(returnedTasks, Is.Not.Null);
        Assert.That(returnedTasks, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectTask1 = new Common.Data.Models.ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        var projectTask2 = new Common.Data.Models.ProjectTaskModel
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.High,
            Type = Common.Enums.Project.TaskType.Bug,
            Order = 2,
            EstimatePoints = 8,
            DueDate = DateTime.UtcNow.AddDays(14),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.AddRange(projectTask1, projectTask2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var page = 1;
        var perPage = 10;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetAsync(page, perPage, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedTasks = okResult.Value as List<GetAllProjectTaskResponse>;
        Assert.That(returnedTasks, Is.Not.Null);
        Assert.That(returnedTasks, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectTask = new Common.Data.Models.ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(projectTask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectTaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedTask = okResult.Value as GetProjectTaskByIdResponse;
        Assert.That(returnedTask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedTask.Id, Is.EqualTo(this.testProjectTaskId));
            Assert.That(returnedTask.Title, Is.EqualTo(projectTask.Title));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedTask = createdResult.Value as AddProjectTaskResponse;
        Assert.That(returnedTask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedTask.Id, Is.EqualTo(command.Id));
            Assert.That(returnedTask.Title, Is.EqualTo(command.Title));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectTask = new Common.Data.Models.ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(projectTask);
        await this.context.SaveChangesAsync(CancellationToken.None);

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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectTaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedTask = okResult.Value as UpdateProjectTaskResponse;
        Assert.That(returnedTask, Is.Not.Null);
        Assert.That(returnedTask.Title, Contains.Substring("Updated"));
    }

    [Test]
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var projectTask = new Common.Data.Models.ProjectTaskModel
        {
            Id = this.testProjectTaskId,
            ProjectId = Guid.NewGuid(),
            StatusId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Priority = Common.Enums.Project.TaskPriority.Medium,
            Type = Common.Enums.Project.TaskType.Task,
            Order = 1,
            EstimatePoints = 5,
            DueDate = DateTime.UtcNow.AddDays(7),
            CreatedBy = Guid.NewGuid()
        };

        this.context.ProjectTasks.Add(projectTask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectTaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project task was deleted
        var deletedTask = await this.context.ProjectTasks.FirstOrDefaultAsync(x => x.Id == this.testProjectTaskId && x.Deleted == null, cancellationToken);
        Assert.That(deletedTask, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void Controller_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectTaskController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectTaskController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectTaskController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "DeleteAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    [Test]
    public void PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PostAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    [Test]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

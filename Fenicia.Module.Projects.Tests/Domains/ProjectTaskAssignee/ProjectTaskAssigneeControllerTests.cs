using System.Security.Claims;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
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

[TestFixture]
public class ProjectTaskAssigneeControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectTaskAssigneeId = Guid.NewGuid();
        this.getAllProjectTaskAssigneeHandler = new GetAllProjectTaskAssigneeHandler(this.context);
        this.getProjectTaskAssigneeByIdHandler = new GetProjectTaskAssigneeByIdHandler(this.context);
        this.addProjectTaskAssigneeHandler = new AddProjectTaskAssigneeHandler(this.context);
        this.updateProjectTaskAssigneeHandler = new UpdateProjectTaskAssigneeHandler(this.context);
        this.deleteProjectTaskAssigneeHandler = new DeleteProjectTaskAssigneeHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectTaskAssigneeController(
            this.getAllProjectTaskAssigneeHandler,
            this.getProjectTaskAssigneeByIdHandler,
            this.addProjectTaskAssigneeHandler,
            this.updateProjectTaskAssigneeHandler,
            this.deleteProjectTaskAssigneeHandler)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = this.mockHttpContext.Object
            }
        };

        SetupUserClaims();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private ProjectTaskAssigneeController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectTaskAssigneeHandler getAllProjectTaskAssigneeHandler = null!;
    private GetProjectTaskAssigneeByIdHandler getProjectTaskAssigneeByIdHandler = null!;
    private AddProjectTaskAssigneeHandler addProjectTaskAssigneeHandler = null!;
    private UpdateProjectTaskAssigneeHandler updateProjectTaskAssigneeHandler = null!;
    private DeleteProjectTaskAssigneeHandler deleteProjectTaskAssigneeHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectTaskAssigneeId;

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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.That(returnedAssignees, Is.Not.Null);
        Assert.That(returnedAssignees, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectTaskAssignee1 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role  = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        var projectTaskAssignee2 = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role  = Common.Enums.Project.AssigneeRole.Contributor,
            AssignedAt = DateTime.UtcNow.AddDays(-1)
        };

        this.context.ProjectTaskAssignees.AddRange(projectTaskAssignee1, projectTaskAssignee2);
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

        var returnedAssignees = okResult.Value as List<GetAllProjectTaskAssigneeResponse>;
        Assert.That(returnedAssignees, Is.Not.Null);
        Assert.That(returnedAssignees, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectTaskAssignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role  = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        this.context.ProjectTaskAssignees.Add(projectTaskAssignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectTaskAssigneeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedAssignee = okResult.Value as GetProjectTaskAssigneeByIdResponse;
        Assert.That(returnedAssignee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedAssignee.Id, Is.EqualTo(this.testProjectTaskAssigneeId));
            Assert.That(returnedAssignee.TaskId, Is.EqualTo(projectTaskAssignee.TaskId));
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
        var command = new AddProjectTaskAssigneeCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Owner",
            DateTime.UtcNow);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedAssignee = createdResult.Value as AddProjectTaskAssigneeResponse;
        Assert.That(returnedAssignee, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedAssignee.Id, Is.EqualTo(command.Id));
            Assert.That(returnedAssignee.TaskId, Is.EqualTo(command.TaskId));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectTaskAssignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role  = Common.Enums.Project.AssigneeRole.Owner,
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

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectTaskAssigneeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedAssignee = okResult.Value as UpdateProjectTaskAssigneeResponse;
        Assert.That(returnedAssignee, Is.Not.Null);
        Assert.That(returnedAssignee.Role, Is.EqualTo("Contributor"));
    }

    [Test]
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
        var projectTaskAssignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = this.testProjectTaskAssigneeId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Role  = Common.Enums.Project.AssigneeRole.Owner,
            AssignedAt = DateTime.UtcNow
        };

        this.context.ProjectTaskAssignees.Add(projectTaskAssignee);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectTaskAssigneeId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project task assignee was deleted
        var deletedAssignee = await this.context.ProjectTaskAssignees.FirstOrDefaultAsync(x => x.Id == this.testProjectTaskAssigneeId && x.Deleted == null, cancellationToken);
        Assert.That(deletedAssignee, Is.Null);
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
        var controllerType = typeof(ProjectTaskAssigneeController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectTaskAssigneeController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskAssigneeController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectTaskAssigneeController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskAssigneeController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectTaskAssigneeController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectTaskAssigneeController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.DeleteAsync));

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
        var controllerType = typeof(ProjectTaskAssigneeController);
        var postMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PostAsync));

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
        var controllerType = typeof(ProjectTaskAssigneeController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectTaskAssigneeController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
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

[TestFixture]
public class ProjectSubtaskControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectSubtaskId = Guid.NewGuid();
        this.getAllProjectSubtaskHandler = new GetAllProjectSubtaskHandler(this.context);
        this.getProjectSubtaskByIdHandler = new GetProjectSubtaskByIdHandler(this.context);
        this.addProjectSubtaskHandler = new AddProjectSubtaskHandler(this.context);
        this.updateProjectSubtaskHandler = new UpdateProjectSubtaskHandler(this.context);
        this.deleteProjectSubtaskHandler = new DeleteProjectSubtaskHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectSubtaskController(
            this.getAllProjectSubtaskHandler,
            this.getProjectSubtaskByIdHandler,
            this.addProjectSubtaskHandler,
            this.updateProjectSubtaskHandler,
            this.deleteProjectSubtaskHandler)
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
    private ProjectSubtaskController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectSubtaskHandler getAllProjectSubtaskHandler = null!;
    private GetProjectSubtaskByIdHandler getProjectSubtaskByIdHandler = null!;
    private AddProjectSubtaskHandler addProjectSubtaskHandler = null!;
    private UpdateProjectSubtaskHandler updateProjectSubtaskHandler = null!;
    private DeleteProjectSubtaskHandler deleteProjectSubtaskHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectSubtaskId;
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

        var returnedSubtasks = okResult.Value as List<GetAllProjectSubtaskResponse>;
        Assert.That(returnedSubtasks, Is.Not.Null);
        Assert.That(returnedSubtasks, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectSubtask1 = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        var projectSubtask2 = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = true,
            Order = 2,
            CompletedAt = DateTime.UtcNow
        };

        this.context.ProjectSubtasks.AddRange(projectSubtask1, projectSubtask2);
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

        var returnedSubtasks = okResult.Value as List<GetAllProjectSubtaskResponse>;
        Assert.That(returnedSubtasks, Is.Not.Null);
        Assert.That(returnedSubtasks, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectSubtask = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = this.testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.Add(projectSubtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectSubtaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSubtask = okResult.Value as GetProjectSubtaskByIdResponse;
        Assert.That(returnedSubtask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSubtask.Id, Is.EqualTo(this.testProjectSubtaskId));
            Assert.That(returnedSubtask.Title, Is.EqualTo(projectSubtask.Title));
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
        var command = new AddProjectSubtaskCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.PickRandom(true, false),
            this.faker.Random.Int(1, 10),
            this.faker.PickRandom<DateTime?>(null, DateTime.UtcNow));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedSubtask = createdResult.Value as AddProjectSubtaskResponse;
        Assert.That(returnedSubtask, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedSubtask.Id, Is.EqualTo(command.Id));
            Assert.That(returnedSubtask.Title, Is.EqualTo(command.Title));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectSubtask = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = this.testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.Add(projectSubtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectSubtaskCommand(
            projectSubtask.Id,
            projectSubtask.TaskId,
            this.faker.Lorem.Sentence(5) + " Updated",
            true,
            projectSubtask.Order,
            DateTime.UtcNow);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectSubtaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedSubtask = okResult.Value as UpdateProjectSubtaskResponse;
        Assert.That(returnedSubtask, Is.Not.Null);
        Assert.That(returnedSubtask.Title, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectSubtaskCommand(
            nonExistentId,
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.PickRandom(true, false),
            this.faker.Random.Int(1, 10),
            this.faker.PickRandom<DateTime?>(null, DateTime.UtcNow));

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
        var projectSubtask = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = this.testProjectSubtaskId,
            TaskId = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            IsCompleted = false,
            Order = 1,
            CompletedAt = null
        };

        this.context.ProjectSubtasks.Add(projectSubtask);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectSubtaskId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project subtask was deleted
        var deletedSubtask = await this.context.ProjectSubtasks.FirstOrDefaultAsync(x => x.Id == this.testProjectSubtaskId && x.Deleted == null, cancellationToken);
        Assert.That(deletedSubtask, Is.Null);
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
        var controllerType = typeof(ProjectSubtaskController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectSubtaskController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectSubtaskController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectSubtaskController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectSubtaskController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectSubtaskController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectSubtaskController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.DeleteAsync));

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
        var controllerType = typeof(ProjectSubtaskController);
        var postMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.PostAsync));

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
        var controllerType = typeof(ProjectSubtaskController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectSubtaskController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
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

[TestFixture]
public class ProjectStatusControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectStatusId = Guid.NewGuid();
        this.getAllProjectStatusHandler = new GetAllProjectStatusHandler(this.context);
        this.getProjectStatusByIdHandler = new GetProjectStatusByIdHandler(this.context);
        this.addProjectStatusHandler = new AddProjectStatusHandler(this.context);
        this.updateProjectStatusHandler = new UpdateProjectStatusHandler(this.context);
        this.deleteProjectStatusHandler = new DeleteProjectStatusHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectStatusController(
            this.getAllProjectStatusHandler,
            this.getProjectStatusByIdHandler,
            this.addProjectStatusHandler,
            this.updateProjectStatusHandler,
            this.deleteProjectStatusHandler)
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
    private ProjectStatusController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectStatusHandler getAllProjectStatusHandler = null!;
    private GetProjectStatusByIdHandler getProjectStatusByIdHandler = null!;
    private AddProjectStatusHandler addProjectStatusHandler = null!;
    private UpdateProjectStatusHandler updateProjectStatusHandler = null!;
    private DeleteProjectStatusHandler deleteProjectStatusHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectStatusId;
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

        var returnedStatuses = okResult.Value as List<GetAllProjectStatusResponse>;
        Assert.That(returnedStatuses, Is.Not.Null);
        Assert.That(returnedStatuses, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectStatus1 = new Common.Data.Models.ProjectStatus
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        var projectStatus2 = new Common.Data.Models.ProjectStatus
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid(),
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 2,
            IsFinal = true
        };

        this.context.ProjectStatuses.AddRange(projectStatus1, projectStatus2);
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

        var returnedStatuses = okResult.Value as List<GetAllProjectStatusResponse>;
        Assert.That(returnedStatuses, Is.Not.Null);
        Assert.That(returnedStatuses, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectStatus = new Common.Data.Models.ProjectStatus
        {
            Id = this.testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        this.context.ProjectStatuses.Add(projectStatus);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectStatusId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedStatus = okResult.Value as GetProjectStatusByIdResponse;
        Assert.That(returnedStatus, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedStatus.Id, Is.EqualTo(this.testProjectStatusId));
            Assert.That(returnedStatus.Name, Is.EqualTo(projectStatus.Name));
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
        var command = new AddProjectStatusCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Int(1, 10),
            this.faker.PickRandom(true, false));

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedStatus = createdResult.Value as AddProjectStatusResponse;
        Assert.That(returnedStatus, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedStatus.Id, Is.EqualTo(command.Id));
            Assert.That(returnedStatus.Name, Is.EqualTo(command.Name));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectStatus = new Common.Data.Models.ProjectStatus
        {
            Id = this.testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        this.context.ProjectStatuses.Add(projectStatus);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(
            projectStatus.Id,
            projectStatus.ProjectId,
            this.faker.Lorem.Word() + " Updated",
            this.faker.Internet.Color(),
            projectStatus.Order,
            projectStatus.IsFinal);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectStatusId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedStatus = okResult.Value as UpdateProjectStatusResponse;
        Assert.That(returnedStatus, Is.Not.Null);
        Assert.That(returnedStatus.Name, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectStatusCommand(
            nonExistentId,
            Guid.NewGuid(),
            this.faker.Lorem.Word(),
            this.faker.Internet.Color(),
            this.faker.Random.Int(1, 10),
            this.faker.PickRandom(true, false));

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
        var projectStatus = new Common.Data.Models.ProjectStatus
        {
            Id = this.testProjectStatusId,
            ProjectId = Guid.NewGuid(),
            Name = this.faker.Lorem.Word(),
            Color = this.faker.Internet.Color(),
            Order = 1,
            IsFinal = false
        };

        this.context.ProjectStatuses.Add(projectStatus);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectStatusId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project status was deleted
        var deletedStatus = await this.context.ProjectStatuses.FirstOrDefaultAsync(x => x.Id == this.testProjectStatusId && x.Deleted == null, cancellationToken);
        Assert.That(deletedStatus, Is.Null);
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
        var controllerType = typeof(ProjectStatusController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectStatusController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectStatusController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectStatusController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectStatusController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectStatusController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectStatusController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectStatusController.DeleteAsync));

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
        var controllerType = typeof(ProjectStatusController);
        var postMethod = controllerType.GetMethod(nameof(ProjectStatusController.PostAsync));

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
        var controllerType = typeof(ProjectStatusController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectStatusController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

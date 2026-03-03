using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.Add;
using Fenicia.Module.Projects.Domains.Project.Delete;
using Fenicia.Module.Projects.Domains.Project.GetAll;
using Fenicia.Module.Projects.Domains.Project.GetById;
using Fenicia.Module.Projects.Domains.Project.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.Project;

[TestFixture]
public class ProjectControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectId = Guid.NewGuid();
        this.getAllProjectHandler = new GetAllProjectHandler(this.context);
        this.getProjectByIdHandler = new GetProjectByIdHandler(this.context);
        this.addProjectHandler = new AddProjectHandler(this.context);
        this.updateProjectHandler = new UpdateProjectHandler(this.context);
        this.deleteProjectHandler = new DeleteProjectHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectController(
            this.getAllProjectHandler,
            this.getProjectByIdHandler,
            this.addProjectHandler,
            this.updateProjectHandler,
            this.deleteProjectHandler)
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
    private ProjectController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectHandler getAllProjectHandler = null!;
    private GetProjectByIdHandler getProjectByIdHandler = null!;
    private AddProjectHandler addProjectHandler = null!;
    private UpdateProjectHandler updateProjectHandler = null!;
    private DeleteProjectHandler deleteProjectHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectId;
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
    public async Task GetAsync_WhenNoProjectsExist_ReturnsOkWithEmptyList()
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

        var returnedProjects = okResult.Value as List<GetAllProjectResponse>;
        Assert.That(returnedProjects, Is.Not.Null);
        Assert.That(returnedProjects, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenProjectsExist_ReturnsOkWithProjects()
    {
        // Arrange
        var project1 = new Common.Data.Models.ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        var project2 = new Common.Data.Models.ProjectModel
        {
            Id = Guid.NewGuid(),
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Completed,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.AddRange(project1, project2);
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

        var returnedProjects = okResult.Value as List<GetAllProjectResponse>;
        Assert.That(returnedProjects, Is.Not.Null);
        Assert.That(returnedProjects, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenProjectExists_ReturnsOkWithProject()
    {
        // Arrange
        var project = new Common.Data.Models.ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProject = okResult.Value as GetProjectByIdResponse;
        Assert.That(returnedProject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedProject.Id, Is.EqualTo(this.testProjectId));
            Assert.That(returnedProject.Title, Is.EqualTo(project.Title));
        }
    }

    [Test]
    public async Task GetByIdAsync_WhenProjectDoesNotExist_ReturnsNotFound()
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
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithProject()
    {
        // Arrange
        var command = new AddProjectCommand(
            Guid.NewGuid(),
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
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

        var returnedProject = createdResult.Value as AddProjectResponse;
        Assert.That(returnedProject, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedProject.Id, Is.EqualTo(command.Id));
            Assert.That(returnedProject.Title, Is.EqualTo(command.Title));
        }
    }

    [Test]
    public async Task PatchAsync_WhenProjectExists_ReturnsOkWithUpdatedProject()
    {
        // Arrange
        var project = new Common.Data.Models.ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommand(
            project.Id,
            this.faker.Lorem.Sentence(5) + " Updated",
            this.faker.Lorem.Paragraph(),
            "Completed",
            project.StartDate,
            DateTime.UtcNow,
            project.Owner);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedProject = okResult.Value as UpdateProjectResponse;
        Assert.That(returnedProject, Is.Not.Null);
        Assert.That(returnedProject.Title, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenProjectDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommand(
            nonExistentId,
            this.faker.Lorem.Sentence(5),
            this.faker.Lorem.Paragraph(),
            "Active",
            DateTime.UtcNow,
            null,
            Guid.NewGuid());

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, nonExistentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteAsync_WhenProjectExists_ReturnsNoContent()
    {
        // Arrange
        var project = new Common.Data.Models.ProjectModel
        {
            Id = this.testProjectId,
            Title = this.faker.Lorem.Sentence(5),
            Description = this.faker.Lorem.Paragraph(),
            Status = Common.Enums.Project.ProjectStatus.Active,
            StartDate = DateTime.UtcNow,
            Owner = Guid.NewGuid()
        };

        this.context.Projects.Add(project);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project was deleted
        var deletedProject = await this.context.Projects.FirstOrDefaultAsync(x => x.Id == this.testProjectId && x.Deleted == null, cancellationToken);
        Assert.That(deletedProject, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WhenProjectDoesNotExist_ReturnsNoContent()
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
    public void ProjectController_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectController should have Authorize attribute");
    }

    [Test]
    public void ProjectController_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void ProjectController_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectController should have ApiController attribute");
    }

    [Test]
    public void ProjectController_DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "DeleteAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    [Test]
    public void ProjectController_PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var postMethod = controllerType.GetMethod(nameof(ProjectController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PostAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }

    [Test]
    public void ProjectController_PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

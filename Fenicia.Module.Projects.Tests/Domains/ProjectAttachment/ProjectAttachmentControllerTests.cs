using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Add;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Delete;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetById;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class ProjectAttachmentControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectAttachmentId = Guid.NewGuid();
        this.getAllProjectAttachmentHandler = new GetAllProjectAttachmentHandler(this.context);
        this.getProjectAttachmentByIdHandler = new GetProjectAttachmentByIdHandler(this.context);
        this.addProjectAttachmentHandler = new AddProjectAttachmentHandler(this.context);
        this.updateProjectAttachmentHandler = new UpdateProjectAttachmentHandler(this.context);
        this.deleteProjectAttachmentHandler = new DeleteProjectAttachmentHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectAttachmentController(
            this.getAllProjectAttachmentHandler,
            this.getProjectAttachmentByIdHandler,
            this.addProjectAttachmentHandler,
            this.updateProjectAttachmentHandler,
            this.deleteProjectAttachmentHandler)
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
    private ProjectAttachmentController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectAttachmentHandler getAllProjectAttachmentHandler = null!;
    private GetProjectAttachmentByIdHandler getProjectAttachmentByIdHandler = null!;
    private AddProjectAttachmentHandler addProjectAttachmentHandler = null!;
    private UpdateProjectAttachmentHandler updateProjectAttachmentHandler = null!;
    private DeleteProjectAttachmentHandler deleteProjectAttachmentHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectAttachmentId;
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

        var returnedAttachments = okResult.Value as List<GetAllProjectAttachmentResponse>;
        Assert.That(returnedAttachments, Is.Not.Null);
        Assert.That(returnedAttachments, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectAttachment1 = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var projectAttachment2 = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.AddRange(projectAttachment1, projectAttachment2);
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

        var returnedAttachments = okResult.Value as List<GetAllProjectAttachmentResponse>;
        Assert.That(returnedAttachments, Is.Not.Null);
        Assert.That(returnedAttachments, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectAttachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(projectAttachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectAttachmentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedAttachment = okResult.Value as GetProjectAttachmentByIdResponse;
        Assert.That(returnedAttachment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedAttachment.Id, Is.EqualTo(this.testProjectAttachmentId));
            Assert.That(returnedAttachment.FileName, Is.EqualTo(projectAttachment.FileName));
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
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.System.FileName(),
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid(),
            "application/json");

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedAttachment = createdResult.Value as AddProjectAttachmentResponse;
        Assert.That(returnedAttachment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedAttachment.Id, Is.EqualTo(command.Id));
            Assert.That(returnedAttachment.FileName, Is.EqualTo(command.FileName));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectAttachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(projectAttachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectAttachmentCommand(
            projectAttachment.Id,
            projectAttachment.TaskId,
            this.faker.System.FileName() + "_updated",
            this.faker.Internet.Url(),
            projectAttachment.FileSize,
            projectAttachment.UploadedBy);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectAttachmentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedAttachment = okResult.Value as UpdateProjectAttachmentResponse;
        Assert.That(returnedAttachment, Is.Not.Null);
        Assert.That(returnedAttachment.FileName, Contains.Substring("_updated"));
    }

    [Test]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectAttachmentCommand(
            nonExistentId,
            Guid.NewGuid(),
            this.faker.System.FileName(),
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
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
        var projectAttachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(projectAttachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectAttachmentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project attachment was deleted
        var deletedAttachment = await this.context.ProjectAttachments.FirstOrDefaultAsync(x => x.Id == this.testProjectAttachmentId && x.Deleted == null, cancellationToken);
        Assert.That(deletedAttachment, Is.Null);
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
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectAttachmentController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectAttachmentController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectAttachmentController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.DeleteAsync));

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
        var controllerType = typeof(ProjectAttachmentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PostAsync));

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
        var controllerType = typeof(ProjectAttachmentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

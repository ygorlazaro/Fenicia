using System.Security.Claims;

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.Add;
using Fenicia.Module.Projects.Domains.ProjectComment.Delete;
using Fenicia.Module.Projects.Domains.ProjectComment.GetAll;
using Fenicia.Module.Projects.Domains.ProjectComment.GetById;
using Fenicia.Module.Projects.Domains.ProjectComment.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

[TestFixture]
public class ProjectCommentControllerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.testProjectCommentId = Guid.NewGuid();
        this.getAllProjectCommentHandler = new GetAllProjectCommentHandler(this.context);
        this.getProjectCommentByIdHandler = new GetProjectCommentByIdHandler(this.context);
        this.addProjectCommentHandler = new AddProjectCommentHandler(this.context);
        this.updateProjectCommentHandler = new UpdateProjectCommentHandler(this.context);
        this.deleteProjectCommentHandler = new DeleteProjectCommentHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectCommentController(
            this.getAllProjectCommentHandler,
            this.getProjectCommentByIdHandler,
            this.addProjectCommentHandler,
            this.updateProjectCommentHandler,
            this.deleteProjectCommentHandler)
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
    private ProjectCommentController controller = null!;
    private DefaultContext context = null!;
    private GetAllProjectCommentHandler getAllProjectCommentHandler = null!;
    private GetProjectCommentByIdHandler getProjectCommentByIdHandler = null!;
    private AddProjectCommentHandler addProjectCommentHandler = null!;
    private UpdateProjectCommentHandler updateProjectCommentHandler = null!;
    private DeleteProjectCommentHandler deleteProjectCommentHandler = null!;
    private Mock<HttpContext> mockHttpContext = null!;
    private Guid testProjectCommentId;
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

        var returnedComments = okResult.Value as List<GetAllProjectCommentResponse>;
        Assert.That(returnedComments, Is.Not.Null);
        Assert.That(returnedComments, Is.Empty);
    }

    [Test]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectComment1 = new Common.Data.Models.ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        var projectComment2 = new Common.Data.Models.ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.AddRange(projectComment1, projectComment2);
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

        var returnedComments = okResult.Value as List<GetAllProjectCommentResponse>;
        Assert.That(returnedComments, Is.Not.Null);
        Assert.That(returnedComments, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectComment = new Common.Data.Models.ProjectCommentModel
        {
            Id = this.testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(projectComment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.GetByIdAsync(this.testProjectCommentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedComment = okResult.Value as GetProjectCommentByIdResponse;
        Assert.That(returnedComment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedComment.Id, Is.EqualTo(this.testProjectCommentId));
            Assert.That(returnedComment.Content, Is.EqualTo(projectComment.Content));
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
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PostAsync(command, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<CreatedResult>());

        var createdResult = result.Result as CreatedResult;
        Assert.That(createdResult, Is.Not.Null);
        Assert.That(createdResult.StatusCode, Is.EqualTo(201));

        var returnedComment = createdResult.Value as AddProjectCommentResponse;
        Assert.That(returnedComment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(returnedComment.Id, Is.EqualTo(command.Id));
            Assert.That(returnedComment.Content, Is.EqualTo(command.Content));
        }
    }

    [Test]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectComment = new Common.Data.Models.ProjectCommentModel
        {
            Id = this.testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(projectComment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(
            projectComment.Id,
            this.faker.Lorem.Paragraph() + " Updated");

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.PatchAsync(command, this.testProjectCommentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());

        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);

        var returnedComment = okResult.Value as UpdateProjectCommentResponse;
        Assert.That(returnedComment, Is.Not.Null);
        Assert.That(returnedComment.Content, Contains.Substring("Updated"));
    }

    [Test]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommentCommand(
            nonExistentId,
            this.faker.Lorem.Paragraph());

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
        var projectComment = new Common.Data.Models.ProjectCommentModel
        {
            Id = this.testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(projectComment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await this.controller.DeleteAsync(this.testProjectCommentId, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);

        // Verify project comment was deleted
        var deletedComment = await this.context.ProjectComments.FirstOrDefaultAsync(x => x.Id == this.testProjectCommentId && x.Deleted == null, cancellationToken);
        Assert.That(deletedComment, Is.Null);
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
        var controllerType = typeof(ProjectCommentController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "ProjectCommentController should have Authorize attribute");
    }

    [Test]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.That(routeAttribute, Is.Not.Null, "ProjectCommentController should have Route attribute");
        Assert.That(routeAttribute!.Template, Is.EqualTo("[controller]"));
    }

    [Test]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.That(apiControllerAttribute, Is.Not.Null, "ProjectCommentController should have ApiController attribute");
    }

    [Test]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectCommentController.DeleteAsync));

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
        var controllerType = typeof(ProjectCommentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectCommentController.PostAsync));

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
        var controllerType = typeof(ProjectCommentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectCommentController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.That(authorizeAttribute, Is.Not.Null, "PatchAsync should have Authorize attribute");
        Assert.That(authorizeAttribute!.Roles, Is.EqualTo("Admin"));
    }
}

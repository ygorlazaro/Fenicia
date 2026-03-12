using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
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

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class ProjectAttachmentControllerTests : IDisposable
{
    public ProjectAttachmentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options,
            companyContext);
        this.testProjectAttachmentId = Guid.NewGuid();
        var getAllProjectAttachmentHandler = new GetAllProjectAttachmentHandler(this.db);
        var getProjectAttachmentByIdHandler = new GetProjectAttachmentByIdHandler(this.db);
        var addProjectAttachmentHandler = new AddProjectAttachmentHandler(this.db);
        var updateProjectAttachmentHandler = new UpdateProjectAttachmentHandler(this.db);
        var deleteProjectAttachmentHandler = new DeleteProjectAttachmentHandler(this.db);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectAttachmentController(
            getAllProjectAttachmentHandler,
            getProjectAttachmentByIdHandler,
            addProjectAttachmentHandler,
            updateProjectAttachmentHandler,
            deleteProjectAttachmentHandler)
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

    private readonly ProjectAttachmentController controller;
    private readonly DefaultContext db;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectAttachmentId;
    private readonly Faker faker;

    private void SetupUserClaims()
    {
        var claims = new List<Claim>
        {
            new("userId",
                Guid.NewGuid()
                    .ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims,
            "Test");
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
        var result = await this.controller.GetAsync(wide,
            page,
            perPage,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachments = okResult.Value as List<GetAllProjectAttachmentResponse>;
        Assert.NotNull(returnedAttachments);
        Assert.Empty(returnedAttachments);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {
        // Arrange
        var projectAttachment1 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000,
                1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var projectAttachment2 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000,
                1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.AddRange(projectAttachment1,
            projectAttachment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide,
            page,
            perPage,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachments = okResult.Value as List<GetAllProjectAttachmentResponse>;
        Assert.NotNull(returnedAttachments);
        Assert.Equal(2,
            returnedAttachments.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {
        // Arrange
        var projectAttachment = new AttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000,
                1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(projectAttachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProjectAttachmentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachment = okResult.Value as GetProjectAttachmentByIdResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Equal(this.testProjectAttachmentId,
            returnedAttachment.Id);
        Assert.Equal(projectAttachment.FileName,
            returnedAttachment.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithItem()
    {
        // Arrange
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.System.FileName(),
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000,
                1000000),
            Guid.NewGuid(),
            "application/json");

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201,
            createdResult.StatusCode);

        var returnedAttachment = createdResult.Value as AddProjectAttachmentResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Equal(command.Id,
            returnedAttachment.Id);
        Assert.Equal(command.FileName,
            returnedAttachment.FileName);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {
        // Arrange
        var projectAttachment = new AttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000,
                1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(projectAttachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectAttachmentCommand(
            projectAttachment.Id,
            projectAttachment.TaskId,
            this.faker.System.FileName() + "_updated",
            this.faker.Internet.Url(),
            projectAttachment.FileSize,
            projectAttachment.UploadedBy);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command,
            this.testProjectAttachmentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachment = okResult.Value as UpdateProjectAttachmentResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Contains("_updated",
            returnedAttachment.FileName);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectAttachmentCommand(
            nonExistentId,
            Guid.NewGuid(),
            this.faker.System.FileName(),
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000,
                1000000),
            Guid.NewGuid());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command,
            nonExistentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var projectAttachment = new AttachmentModel
        {
            Id = this.testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000,
                1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(projectAttachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProjectAttachmentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);

        // Verify project attachment was deleted
        var deletedAttachment = await this.db.ProjectAttachments.FirstOrDefaultAsync(
            x => x.Id == this.testProjectAttachmentId && x.Deleted == null,
            ct);
        Assert.Null(deletedAttachment);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(nonExistentId,
            wide,
            ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Controller_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute),
                false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]",
            routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute),
                false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin",
            authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin",
            authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectAttachmentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute),
            false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin",
            authorizeAttribute.Roles);
    }
}

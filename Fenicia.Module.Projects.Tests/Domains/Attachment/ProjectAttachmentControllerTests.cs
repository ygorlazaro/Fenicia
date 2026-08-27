using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
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
    private readonly ProjectAttachmentController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectAttachmentId;

    public ProjectAttachmentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectAttachmentId = Guid.NewGuid();
        var getAllProjectAttachmentHandler = new GetAllProjectAttachmentHandler(db);
        var getProjectAttachmentByIdHandler = new GetProjectAttachmentByIdHandler(db);
        var addProjectAttachmentHandler = new AddProjectAttachmentHandler(db);
        var updateProjectAttachmentHandler = new UpdateProjectAttachmentHandler(db);
        var deleteProjectAttachmentHandler = new DeleteProjectAttachmentHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectAttachmentController(getAllProjectAttachmentHandler, getProjectAttachmentByIdHandler, addProjectAttachmentHandler, updateProjectAttachmentHandler, deleteProjectAttachmentHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

        SetupUserClaims();
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SetupUserClaims()
    {
        var claims = new List<Claim> { new("userId", Guid.NewGuid().ToString()) };

        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }

    [Fact]
    public async Task GetAsync_WhenNoItemsExist_ReturnsOkWithEmptyList()
    {

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

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

        var projectAttachment1 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = faker.System.FileName(),
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var projectAttachment2 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = faker.System.FileName(),
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.AddRange(projectAttachment1, projectAttachment2);
        await db.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetAsync(wide, page, perPage, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachments = okResult.Value as List<GetAllProjectAttachmentResponse>;
        Assert.NotNull(returnedAttachments);
        Assert.Equal(2, returnedAttachments.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectAttachment = new AttachmentModel
        {
            Id = testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = faker.System.FileName(),
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.Add(projectAttachment);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectAttachmentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachment = okResult.Value as GetProjectAttachmentByIdResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Equal(testProjectAttachmentId, returnedAttachment.Id);
        Assert.Equal(projectAttachment.FileName, returnedAttachment.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithItem()
    {

        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), faker.System.FileName(), faker.Internet.Url(), faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedAttachment = createdResult.Value as AddProjectAttachmentResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Equal(command.Id, returnedAttachment.Id);
        Assert.Equal(command.FileName, returnedAttachment.FileName);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectAttachment = new AttachmentModel
        {
            Id = testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = faker.System.FileName(),
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.Add(projectAttachment);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectAttachmentCommand(projectAttachment.Id, projectAttachment.TaskId, faker.System.FileName() + "_updated", faker.Internet.Url(), projectAttachment.FileSize, projectAttachment.UploadedBy);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectAttachmentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedAttachment = okResult.Value as UpdateProjectAttachmentResponse;
        Assert.NotNull(returnedAttachment);
        Assert.Contains("_updated", returnedAttachment.FileName);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectAttachmentCommand(nonExistentId, Guid.NewGuid(), faker.System.FileName(), faker.Internet.Url(), faker.Random.Long(1000, 1000000), Guid.NewGuid());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectAttachment = new AttachmentModel
        {
            Id = testProjectAttachmentId,
            TaskId = Guid.NewGuid(),
            FileName = faker.System.FileName(),
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.Add(projectAttachment);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectAttachmentId, wide, ct);

        Assert.NotNull(result);

        var deletedAttachment = await db.ProjectAttachments.FirstOrDefaultAsync(x => x.Id == testProjectAttachmentId && x.Deleted == null, ct);
        Assert.Null(deletedAttachment);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
    {

        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(nonExistentId, wide, ct);

        Assert.NotNull(result);
    }

    [Fact]
    public void Controller_HasAuthorizeAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectAttachmentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectAttachmentController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

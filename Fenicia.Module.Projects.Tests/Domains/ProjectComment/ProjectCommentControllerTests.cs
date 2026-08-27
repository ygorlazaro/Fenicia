using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
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

public class ProjectCommentControllerTests : IDisposable
{
    private readonly ProjectCommentController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectCommentId;

    public ProjectCommentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        testProjectCommentId = Guid.NewGuid();
        var getAllProjectCommentHandler = new GetAllProjectCommentHandler(db);
        var getProjectCommentByIdHandler = new GetProjectCommentByIdHandler(db);
        var addProjectCommentHandler = new AddProjectCommentHandler(db);
        var updateProjectCommentHandler = new UpdateProjectCommentHandler(db);
        var deleteProjectCommentHandler = new DeleteProjectCommentHandler(db);
        mockHttpContext = new Mock<HttpContext>();

        controller = new ProjectCommentController(getAllProjectCommentHandler, getProjectCommentByIdHandler, addProjectCommentHandler, updateProjectCommentHandler, deleteProjectCommentHandler) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };

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

        var returnedComments = okResult.Value as List<GetAllProjectCommentResponse>;
        Assert.NotNull(returnedComments);
        Assert.Empty(returnedComments);
    }

    [Fact]
    public async Task GetAsync_WhenItemsExist_ReturnsOkWithItems()
    {

        var projectComment1 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = faker.Lorem.Paragraph()
        };

        var projectComment2 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.AddRange(projectComment1, projectComment2);
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

        var returnedComments = okResult.Value as List<GetAllProjectCommentResponse>;
        Assert.NotNull(returnedComments);
        Assert.Equal(2, returnedComments.Count);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemExists_ReturnsOkWithItem()
    {

        var projectComment = new ProjectCommentModel
        {
            Id = testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.Add(projectComment);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.GetByIdAsync(testProjectCommentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedComment = okResult.Value as GetProjectCommentByIdResponse;
        Assert.NotNull(returnedComment);
        Assert.Equal(testProjectCommentId, returnedComment.Id);
        Assert.Equal(projectComment.Content, returnedComment.Content);
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

        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), faker.Lorem.Paragraph());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PostAsync(command, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<CreatedResult>(result.Result);

        var createdResult = result.Result as CreatedResult;
        Assert.NotNull(createdResult);
        Assert.Equal(201, createdResult.StatusCode);

        var returnedComment = createdResult.Value as AddProjectCommentResponse;
        Assert.NotNull(returnedComment);
        Assert.Equal(command.Id, returnedComment.Id);
        Assert.Equal(command.Content, returnedComment.Content);
    }

    [Fact]
    public async Task PatchAsync_WhenItemExists_ReturnsOkWithUpdatedItem()
    {

        var projectComment = new ProjectCommentModel
        {
            Id = testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.Add(projectComment);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(projectComment.Id, faker.Lorem.Paragraph() + " Updated");

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, testProjectCommentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedComment = okResult.Value as UpdateProjectCommentResponse;
        Assert.NotNull(returnedComment);
        Assert.Contains("Updated", returnedComment.Content);
    }

    [Fact]
    public async Task PatchAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {

        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommentCommand(nonExistentId, faker.Lorem.Paragraph());

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.PatchAsync(command, nonExistentId, wide, ct);

        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {

        var projectComment = new ProjectCommentModel
        {
            Id = testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.Add(projectComment);
        await db.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        var wide = new WideEventContext();
        var result = await controller.DeleteAsync(testProjectCommentId, wide, ct);

        Assert.NotNull(result);

        var deletedComment = await db.ProjectComments.FirstOrDefaultAsync(x => x.Id == testProjectCommentId && x.Deleted == null, ct);
        Assert.Null(deletedComment);
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

        var controllerType = typeof(ProjectCommentController);

        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {

        var controllerType = typeof(ProjectCommentController);

        var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {

        var controllerType = typeof(ProjectCommentController);

        var apiControllerAttribute = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectCommentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectCommentController.DeleteAsync));

        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectCommentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectCommentController.PostAsync));

        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {

        var controllerType = typeof(ProjectCommentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectCommentController.PatchAsync));

        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

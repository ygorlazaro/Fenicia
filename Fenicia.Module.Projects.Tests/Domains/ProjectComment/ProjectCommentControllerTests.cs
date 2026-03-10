using System.Security.Claims;

using Bogus;

using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
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
    public ProjectCommentControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.testProjectCommentId = Guid.NewGuid();
        var getAllProjectCommentHandler = new GetAllProjectCommentHandler(this.context);
        var getProjectCommentByIdHandler = new GetProjectCommentByIdHandler(this.context);
        var addProjectCommentHandler = new AddProjectCommentHandler(this.context);
        var updateProjectCommentHandler = new UpdateProjectCommentHandler(this.context);
        var deleteProjectCommentHandler = new DeleteProjectCommentHandler(this.context);
        this.mockHttpContext = new Mock<HttpContext>();

        this.controller = new ProjectCommentController(
            getAllProjectCommentHandler,
            getProjectCommentByIdHandler,
            addProjectCommentHandler,
            updateProjectCommentHandler,
            deleteProjectCommentHandler)
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
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly ProjectCommentController controller;
    private readonly DefaultContext context;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testProjectCommentId;
    private readonly Faker faker;

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

    [Fact]
    public async Task GetAsync_WhenNoItemsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
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
        // Arrange
        var projectComment1 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        var projectComment2 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.AddRange(projectComment1, projectComment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        const int page = 1;
        const int perPage = 10;
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetAsync(wide, page, perPage, ct);

        // Assert
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
        // Arrange
        var projectComment = new ProjectCommentModel
        {
            Id = this.testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(projectComment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(this.testProjectCommentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result.Result);

        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var returnedComment = okResult.Value as GetProjectCommentByIdResponse;
        Assert.NotNull(returnedComment);
        Assert.Equal(this.testProjectCommentId, returnedComment.Id);
        Assert.Equal(projectComment.Content, returnedComment.Content);
    }

    [Fact]
    public async Task GetByIdAsync_WhenItemDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.GetByIdAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostAsync_WithValidCommand_ReturnsCreatedWithItem()
    {
        // Arrange
        var command = new AddProjectCommentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            this.faker.Lorem.Paragraph());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PostAsync(command, wide, ct);

        // Assert
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
        // Arrange
        var projectComment = new ProjectCommentModel
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

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, this.testProjectCommentId, wide, ct);

        // Assert
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
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var command = new UpdateProjectCommentCommand(
            nonExistentId,
            this.faker.Lorem.Paragraph());

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.PatchAsync(command, nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemExists_ReturnsNoContent()
    {
        // Arrange
        var projectComment = new ProjectCommentModel
        {
            Id = this.testProjectCommentId,
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = this.faker.Lorem.Paragraph()
        };

        this.context.ProjectComments.Add(projectComment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(this.testProjectCommentId, wide, ct);

        // Assert
        Assert.NotNull(result);

        // Verify project comment was deleted
        var deletedComment = await this.context.ProjectComments.FirstOrDefaultAsync(x => x.Id == this.testProjectCommentId && x.Deleted == null, ct);
        Assert.Null(deletedComment);
    }

    [Fact]
    public async Task DeleteAsync_WhenItemDoesNotExist_ReturnsNoContent()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var ct = CancellationToken.None;

        // Act
        var wide = new WideEventContext();
        var result = await this.controller.DeleteAsync(nonExistentId, wide, ct);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Controller_HasAuthorizeAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);

        // Act
        var authorizeAttribute = controllerType.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(authorizeAttribute);
    }

    [Fact]
    public void Controller_HasRouteAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);

        // Act
        var routeAttribute =
            controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;

        // Assert
        Assert.NotNull(routeAttribute);
        Assert.Equal("[controller]", routeAttribute.Template);
    }

    [Fact]
    public void Controller_HasApiControllerAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);

        // Act
        var apiControllerAttribute =
            controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault();

        // Assert
        Assert.NotNull(apiControllerAttribute);
    }

    [Fact]
    public void DeleteAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);
        var deleteMethod = controllerType.GetMethod(nameof(ProjectCommentController.DeleteAsync));

        // Act
        var authorizeAttribute = deleteMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PostAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);
        var postMethod = controllerType.GetMethod(nameof(ProjectCommentController.PostAsync));

        // Act
        var authorizeAttribute = postMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }

    [Fact]
    public void PatchAction_HasAuthorizeAdminAttribute()
    {
        // Arrange
        var controllerType = typeof(ProjectCommentController);
        var patchMethod = controllerType.GetMethod(nameof(ProjectCommentController.PatchAsync));

        // Act
        var authorizeAttribute = patchMethod?.GetCustomAttributes(typeof(AuthorizeAttribute), false).FirstOrDefault() as AuthorizeAttribute;

        // Assert
        Assert.NotNull(authorizeAttribute);
        Assert.Equal("Admin", authorizeAttribute.Roles);
    }
}

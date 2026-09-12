using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class ProjectCommentServiceTests
{
    private readonly DbContextOptions<DefaultContext> _dbOptions = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    private readonly Faker _faker = new();
    private readonly Mock<ICompanyContext> _mockCompanyContext = new();

    [Fact]
    public async Task GetAllAsync_WhenCommentsExist_ReturnsComments()
    {
        var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User" };
        var comments = new List<ProjectCommentModel>
        {
            new()
            {
                Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = user.Id,
                Content = _faker.Lorem.Sentence(), CompanyId = Guid.NewGuid(), User = user
            }
        };

        var db = NewDb();
        db.AuthUsers.Add(user);
        db.ProjectComments.AddRange(comments);
        await db.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(db);

        var result = await service.GetAllAsync(new GetAllProjectCommentQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(comments[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User" };
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = user.Id, Content = "hello",
            CompanyId = Guid.NewGuid(), User = user
        };

        var db = NewDb();
        db.AuthUsers.Add(user);
        db.ProjectComments.Add(comment);
        await db.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(db);

        var result = await service.GetByIdAsync(new GetProjectCommentByIdQuery(comment.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(comment.Id);
        result.Content.Should().Be("hello");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        var db = NewDb();
        var service = CreateService(db);

        var result = await service.GetByIdAsync(
            new GetProjectCommentByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedComment()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "new comment");

        var db = NewDb();
        var service = CreateService(db);

        var result = await service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentExists_ReturnsUpdatedComment()
    {
        var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User" };
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = user.Id, Content = "updated",
            CompanyId = Guid.NewGuid(), User = user
        };

        var db = NewDb();
        db.AuthUsers.Add(user);
        db.ProjectComments.Add(comment);
        await db.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(db);

        var command = new UpdateProjectCommentCommand(comment.Id, "updated");

        var result = await service.UpdateAsync(command, comment.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Content.Should().Be("updated");
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        var db = NewDb();
        var service = CreateService(db);

        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), "x");

        var result = await service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var db = NewDb();
        var service = CreateService(db);
        var id = Guid.NewGuid();

        return service.DeleteAsync(new DeleteProjectCommentCommand(id), CancellationToken.None);
    }

    private static ProjectCommentService CreateService(DefaultContext db)
    {
        return new ProjectCommentService(new ProjectCommentRepository(db));
    }

    private DefaultContext NewDb()
    {
        return new DefaultContext(_dbOptions, _mockCompanyContext.Object);
    }
}

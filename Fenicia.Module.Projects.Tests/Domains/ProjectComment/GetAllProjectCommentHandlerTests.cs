using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class GetAllProjectCommentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectCommentHandler handler;

    public GetAllProjectCommentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetAllProjectCommentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectComments_ReturnsAllProjectComments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment1 = new ProjectCommentModel { Id = Guid.NewGuid(), TaskId = taskId, UserId = userId, Content = this.faker.Lorem.Paragraph() };

        var comment2 = new ProjectCommentModel { Id = Guid.NewGuid(), TaskId = taskId, UserId = userId, Content = this.faker.Lorem.Paragraph() };

        this.db.ProjectComments.AddRange(comment1, comment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(comment1.Id, result[0].Id);
        Assert.Equal(comment2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new ProjectCommentModel { Id = Guid.NewGuid(), TaskId = taskId, UserId = userId, Content = $"{this.faker.Lorem.Paragraph()} {i}" };
            this.db.ProjectComments.Add(comment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var comment = new ProjectCommentModel { Id = Guid.NewGuid(), TaskId = taskId, UserId = userId, Content = $"{this.faker.Lorem.Paragraph()} {i}" };
            this.db.ProjectComments.Add(comment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new ProjectCommentModel { Id = Guid.NewGuid(), TaskId = taskId, UserId = userId, Content = $"{this.faker.Lorem.Paragraph()} {i}" };
            this.db.ProjectComments.Add(comment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}
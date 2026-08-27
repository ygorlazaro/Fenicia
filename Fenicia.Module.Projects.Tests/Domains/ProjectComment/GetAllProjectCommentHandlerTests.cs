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
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProjectCommentHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {

        var query = new GetAllProjectCommentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectComments_ReturnsAllProjectComments()
    {

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment1 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        var comment2 = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            Content = faker.Lorem.Paragraph()
        };

        db.ProjectComments.AddRange(comment1, comment2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(comment1.Id, result[0].Id);
        Assert.Equal(comment2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new ProjectCommentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{faker.Lorem.Paragraph()} {i}"
            };
            db.ProjectComments.Add(comment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var comment = new ProjectCommentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{faker.Lorem.Paragraph()} {i}"
            };
            db.ProjectComments.Add(comment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var comment = new ProjectCommentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                UserId = userId,
                Content = $"{faker.Lorem.Paragraph()} {i}"
            };
            db.ProjectComments.Add(comment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectCommentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}

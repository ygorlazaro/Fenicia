using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class GetAllProjectAttachmentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectAttachmentHandler handler;

    public GetAllProjectAttachmentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetAllProjectAttachmentHandler(db);
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

        var query = new GetAllProjectAttachmentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectAttachments_ReturnsAllProjectAttachments()
    {

        var taskId = Guid.NewGuid();
        var attachment1 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{faker.System.FileName()}.pdf",
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{faker.System.FileName()}.docx",
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.AddRange(attachment1, attachment2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(attachment1.Id, result[0].Id);
        Assert.Equal(attachment2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {

        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{faker.System.FileName()}_{i}.pdf",
                FileUrl = faker.Internet.Url(),
                FileSize = faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            db.ProjectAttachments.Add(attachment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(2);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {

        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{faker.System.FileName()}_{i}.pdf",
                FileUrl = faker.Internet.Url(),
                FileSize = faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            db.ProjectAttachments.Add(attachment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(10);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {

        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{faker.System.FileName()}_{i}.pdf",
                FileUrl = faker.Internet.Url(),
                FileSize = faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            db.ProjectAttachments.Add(attachment);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}

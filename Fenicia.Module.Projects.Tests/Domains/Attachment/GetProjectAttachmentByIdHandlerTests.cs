using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class GetProjectAttachmentByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetProjectAttachmentByIdHandler handler;

    public GetProjectAttachmentByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProjectAttachmentByIdHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentExists_ReturnsProjectAttachmentResponse()
    {

        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var fileName = $"{faker.System.FileName()}.pdf";
        var fileUrl = faker.Internet.Url();
        var fileSize = faker.Random.Long(1000, 1000000);
        var uploadedBy = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = fileName,
            FileUrl = fileUrl,
            FileSize = fileSize,
            UploadedBy = uploadedBy,
            ContentType = "application/json"
        };

        db.ProjectAttachments.Add(attachment);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(fileName, result.FileName);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_ReturnsNull()
    {

        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectAttachments_ReturnsOnlyRequestedAttachment()
    {

        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new AttachmentModel
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = $"{faker.System.FileName()}.pdf",
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new AttachmentModel
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = $"{faker.System.FileName()}.docx",
            FileUrl = faker.Internet.Url(),
            FileSize = faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.AddRange(attachment1, attachment2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachment1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(attachment1Id, result.Id);
        Assert.Equal(attachment1.FileName, result.FileName);
    }

    [Fact]
    public async Task Handle_WithLargeFileSize_ReturnsCorrectResponse()
    {

        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var largeFileSize = 1073741824L;
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = $"{faker.System.FileName()}.zip",
            FileUrl = faker.Internet.Url(),
            FileSize = largeFileSize,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        db.ProjectAttachments.Add(attachment);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(largeFileSize, result.FileSize);
    }
}

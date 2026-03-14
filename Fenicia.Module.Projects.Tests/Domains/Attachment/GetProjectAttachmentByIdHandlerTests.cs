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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetProjectAttachmentByIdHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentExists_ReturnsProjectAttachmentResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var fileName = $"{this.faker.System.FileName()}.pdf";
        var fileUrl = this.faker.Internet.Url();
        var fileSize = this.faker.Random.Long(1000, 1000000);
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

        this.db.ProjectAttachments.Add(attachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(fileName, result.FileName);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectAttachments_ReturnsOnlyRequestedAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new AttachmentModel
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new AttachmentModel
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachment1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment1Id, result.Id);
        Assert.Equal(attachment1.FileName, result.FileName);
    }

    [Fact]
    public async Task Handle_WithLargeFileSize_ReturnsCorrectResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var largeFileSize = 1073741824L; // 1 GB
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.zip",
            FileUrl = this.faker.Internet.Url(),
            FileSize = largeFileSize,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(attachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(largeFileSize, result.FileSize);
    }
}
using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class UpdateProjectAttachmentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UpdateProjectAttachmentHandler handler;

    public UpdateProjectAttachmentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new UpdateProjectAttachmentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentExists_UpdatesProjectAttachmentAndReturnsResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = "old_file.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(attachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var newFileName = $"{this.faker.System.FileName()}.pdf";
        var newFileUrl = this.faker.Internet.Url();
        var newFileSize = this.faker.Random.Long(50000, 100000);
        var command = new UpdateProjectAttachmentCommand(attachmentId, taskId, newFileName, newFileUrl, newFileSize, Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(newFileName, result.FileName);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{this.faker.System.FileName()}.pdf", this.faker.Internet.Url(), this.faker.Random.Long(1000, 1000000), Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{this.faker.System.FileName()}.pdf", this.faker.Internet.Url(), this.faker.Random.Long(1000, 1000000), Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new AttachmentModel
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = "file1.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new AttachmentModel
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = "file2.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 20000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var newFileName = $"{this.faker.System.FileName()}_updated.pdf";
        var command = new UpdateProjectAttachmentCommand(attachment1Id, taskId, newFileName, this.faker.Internet.Url(), 50000, Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment1Id, result.Id);
        Assert.Equal(newFileName, result.FileName);

        var updatedAttachment1 = await this.db.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var attachment2InDb = await this.db.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);

        Assert.NotNull(updatedAttachment1);
        Assert.NotNull(attachment2InDb);
        Assert.Equal(newFileName, updatedAttachment1.FileName);
        Assert.Equal("file2.docx", attachment2InDb.FileName);
    }

    [Fact]
    public async Task Handle_WithDifferentFileSize_UpdatesProjectAttachmentSuccessfully()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.Add(attachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        const long newFileSize = 500000L;
        var command = new UpdateProjectAttachmentCommand(attachmentId, taskId, "updated_file.pdf", this.faker.Internet.Url(), newFileSize, Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachmentId, result.Id);
        Assert.Equal(newFileSize, result.FileSize);
    }
}
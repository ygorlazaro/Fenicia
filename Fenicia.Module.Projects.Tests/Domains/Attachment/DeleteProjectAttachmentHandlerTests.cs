using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class DeleteProjectAttachmentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DeleteProjectAttachmentHandler handler;

    public DeleteProjectAttachmentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new DeleteProjectAttachmentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentExists_SetsDeletedDate()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/pdf"
        };

        this.db.ProjectAttachments.Add(attachment);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectAttachmentCommand(attachmentId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAttachment = await this.db.ProjectAttachments.FindAsync([attachmentId], CancellationToken.None);
        Assert.NotNull(deletedAttachment);
        Assert.NotNull(deletedAttachment.Deleted);
        Assert.InRange(deletedAttachment.Deleted.Value, beforeDelete.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectAttachmentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachments = await this.db.ProjectAttachments.ToListAsync();
        Assert.Empty(attachments);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectAttachmentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachments = await this.db.ProjectAttachments.ToListAsync();
        Assert.Empty(attachments);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectAttachments_OnlyDeletesSpecified()
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

        var command = new DeleteProjectAttachmentCommand(attachment1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAttachment = await this.db.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var notDeletedAttachment = await this.db.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);

        Assert.NotNull(deletedAttachment);
        Assert.NotNull(deletedAttachment.Deleted);
        Assert.NotNull(notDeletedAttachment);
        Assert.Null(notDeletedAttachment.Deleted);
    }

    [Fact]
    public async Task Handle_WithMultipleProjectAttachments_DeletesCorrectProjectAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var attachment3Id = Guid.NewGuid();
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

        var attachment3 = new AttachmentModel
        {
            Id = attachment3Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.xlsx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.AddRange(attachment1, attachment2, attachment3);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectAttachmentCommand(attachment2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachment1InDb = await this.db.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var deletedAttachment = await this.db.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);
        var attachment3InDb = await this.db.ProjectAttachments.FindAsync([attachment3Id], CancellationToken.None);

        Assert.NotNull(attachment1InDb);
        Assert.NotNull(deletedAttachment);
        Assert.NotNull(attachment3InDb);
        Assert.Null(attachment1InDb.Deleted);
        Assert.NotNull(deletedAttachment.Deleted);
        Assert.Null(attachment3InDb.Deleted);
    }
}
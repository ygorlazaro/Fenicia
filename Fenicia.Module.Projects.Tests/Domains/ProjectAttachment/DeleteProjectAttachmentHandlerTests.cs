using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class DeleteProjectAttachmentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProjectAttachmentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProjectAttachmentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectAttachmentExists_SetsDeletedDate()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new Common.Data.Models.ProjectAttachment
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/pdf",
        };

        this.context.ProjectAttachments.Add(attachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectAttachmentCommand(attachmentId);
        var beforeDelete = DateTime.UtcNow;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAttachment = await this.context.ProjectAttachments.FindAsync([attachmentId], CancellationToken.None);
        Assert.That(deletedAttachment, Is.Not.Null);
        Assert.That(deletedAttachment.Deleted, Is.Not.Null);
        Assert.That(deletedAttachment.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedAttachment.Deleted, Is.LessThanOrEqualTo(DateTime.UtcNow.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectAttachmentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachments = await this.context.ProjectAttachments.ToListAsync();
        Assert.That(attachments, Is.Empty);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProjectAttachmentCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachments = await this.context.ProjectAttachments.ToListAsync();
        Assert.That(attachments, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProjectAttachments_OnlyDeletesSpecified()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new Common.Data.Models.ProjectAttachment
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new Common.Data.Models.ProjectAttachment
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectAttachmentCommand(attachment1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedAttachment = await this.context.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var notDeletedAttachment = await this.context.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);

        Assert.That(deletedAttachment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedAttachment.Deleted, Is.Not.Null);
            Assert.That(notDeletedAttachment, Is.Not.Null);
        }
        Assert.That(notDeletedAttachment?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectAttachments_DeletesCorrectProjectAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var attachment3Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new Common.Data.Models.ProjectAttachment
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new Common.Data.Models.ProjectAttachment
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment3 = new Common.Data.Models.ProjectAttachment
        {
            Id = attachment3Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.xlsx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.AddRange(attachment1, attachment2, attachment3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProjectAttachmentCommand(attachment2Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachment1InDb = await this.context.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var deletedAttachment = await this.context.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);
        var attachment3InDb = await this.context.ProjectAttachments.FindAsync([attachment3Id], CancellationToken.None);

        Assert.That(attachment1InDb, Is.Not.Null);
        Assert.That(deletedAttachment, Is.Not.Null);
        Assert.That(attachment3InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(attachment1InDb.Deleted, Is.Null);
            Assert.That(deletedAttachment.Deleted, Is.Not.Null);
            Assert.That(attachment3InDb.Deleted, Is.Null);
        }
    }
}

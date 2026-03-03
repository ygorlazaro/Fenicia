using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class UpdateProjectAttachmentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProjectAttachmentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProjectAttachmentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectAttachmentExists_UpdatesProjectAttachmentAndReturnsResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = "old_file.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(attachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var newFileName = $"{this.faker.System.FileName()}.pdf";
        var newFileUrl = this.faker.Internet.Url();
        var newFileSize = this.faker.Random.Long(50000, 100000);
        var command = new UpdateProjectAttachmentCommand(
            attachmentId,
            taskId,
            newFileName,
            newFileUrl,
            newFileSize,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachmentId));
            Assert.That(result.FileName, Is.EqualTo(newFileName));
        }
    }

    [Test]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"{this.faker.System.FileName()}.pdf",
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"{this.faker.System.FileName()}.pdf",
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_UpdatesCorrectProjectAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = "file1.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachment2Id,
            TaskId = taskId,
            FileName = "file2.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = 20000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var newFileName = $"{this.faker.System.FileName()}_updated.pdf";
        var command = new UpdateProjectAttachmentCommand(
            attachment1Id,
            taskId,
            newFileName,
            this.faker.Internet.Url(),
            50000,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachment1Id));
            Assert.That(result.FileName, Is.EqualTo(newFileName));
        }

        var updatedAttachment1 = await this.context.ProjectAttachments.FindAsync([attachment1Id], CancellationToken.None);
        var attachment2InDb = await this.context.ProjectAttachments.FindAsync([attachment2Id], CancellationToken.None);

        Assert.That(updatedAttachment1, Is.Not.Null);
        Assert.That(attachment2InDb, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedAttachment1.FileName, Is.EqualTo(newFileName));
            Assert.That(attachment2InDb.FileName, Is.EqualTo("file2.docx"));
        }
    }

    [Test]
    public async Task Handle_WithDifferentFileSize_UpdatesProjectAttachmentSuccessfully()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var attachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = this.faker.System.FileName(),
            FileUrl = this.faker.Internet.Url(),
            FileSize = 10000,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(attachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        const long newFileSize = 500000L;
        var command = new UpdateProjectAttachmentCommand(
            attachmentId,
            taskId,
            "updated_file.pdf",
            this.faker.Internet.Url(),
            newFileSize,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachmentId));
            Assert.That(result.FileSize, Is.EqualTo(newFileSize));
        }
    }
}

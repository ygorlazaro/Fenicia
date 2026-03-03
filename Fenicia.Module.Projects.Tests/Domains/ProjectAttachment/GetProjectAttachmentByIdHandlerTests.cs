using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class GetProjectAttachmentByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetProjectAttachmentByIdHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetProjectAttachmentByIdHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WhenProjectAttachmentExists_ReturnsProjectAttachmentResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var fileName = $"{this.faker.System.FileName()}.pdf";
        var fileUrl = this.faker.Internet.Url();
        var fileSize = this.faker.Random.Long(1000, 1000000);
        var uploadedBy = Guid.NewGuid();
        var attachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = fileName,
            FileUrl = fileUrl,
            FileSize = fileSize,
            UploadedBy = uploadedBy,
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(attachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachmentId));
            Assert.That(result.FileName, Is.EqualTo(fileName));
        }
    }

    [Test]
    public async Task Handle_WhenProjectAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProjectAttachmentByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProjectAttachments_ReturnsOnlyRequestedAttachment()
    {
        // Arrange
        var attachment1Id = Guid.NewGuid();
        var attachment2Id = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        var attachment1 = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachment1Id,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new Common.Data.Models.ProjectAttachmentModel
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

        var query = new GetProjectAttachmentByIdQuery(attachment1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachment1Id));
            Assert.That(result.FileName, Is.EqualTo(attachment1.FileName));
        }
    }

    [Test]
    public async Task Handle_WithLargeFileSize_ReturnsCorrectResponse()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var largeFileSize = 1073741824L; // 1 GB
        var attachment = new Common.Data.Models.ProjectAttachmentModel
        {
            Id = attachmentId,
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.zip",
            FileUrl = this.faker.Internet.Url(),
            FileSize = largeFileSize,
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.Add(attachment);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProjectAttachmentByIdQuery(attachmentId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(attachmentId));
            Assert.That(result.FileSize, Is.EqualTo(largeFileSize));
        }
    }
}

using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class AddProjectAttachmentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new AddProjectAttachmentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private AddProjectAttachmentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProjectAttachmentAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"{this.faker.System.FileName()}.pdf",
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid(),
            "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.FileName, Is.EqualTo(command.FileName));
        }
    }

    [Test]
    public async Task Handle_VerifiesProjectAttachmentWasSaved()
    {
        // Arrange
        var fileName = $"{this.faker.System.FileName()}.pdf";
        var fileUrl = this.faker.Internet.Url();
        var fileSize = this.faker.Random.Long(1000, 1000000);
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            fileName,
            fileUrl,
            fileSize,
            Guid.NewGuid(),
            "application/json");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachment = await this.context.ProjectAttachments
            .FirstOrDefaultAsync(a => a.Id == command.Id);

        Assert.That(attachment, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(attachment.FileName, Is.EqualTo(fileName));
            Assert.That(attachment.FileUrl, Is.EqualTo(fileUrl));
        }
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProjectAttachments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            taskId,
            $"{this.faker.System.FileName()}.pdf",
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid(),
            "application/json");

        var command2 = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            taskId,
            $"{this.faker.System.FileName()}.docx",
            this.faker.Internet.Url(),
            this.faker.Random.Long(1000, 1000000),
            Guid.NewGuid(),
            "application/json");

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var attachments = await this.context.ProjectAttachments.ToListAsync();
        Assert.That(attachments, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithLargeFileSize_AddsProjectAttachmentSuccessfully()
    {
        // Arrange
        var largeFileSize = 1073741824L; // 1 GB
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"{this.faker.System.FileName()}.zip",
            this.faker.Internet.Url(),
            largeFileSize,
            Guid.NewGuid(),
            "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.FileSize, Is.EqualTo(largeFileSize));
        }
    }

    [Test]
    public async Task Handle_WithSmallFileSize_AddsProjectAttachmentSuccessfully()
    {
        // Arrange
        var smallFileSize = 100L;
        var command = new AddProjectAttachmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            $"{this.faker.System.FileName()}.txt",
            this.faker.Internet.Url(),
            smallFileSize,
            Guid.NewGuid(),
            "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.FileSize, Is.EqualTo(smallFileSize));
        }
    }
}

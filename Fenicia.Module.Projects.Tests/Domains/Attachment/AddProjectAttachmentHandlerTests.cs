using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class AddProjectAttachmentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly AddProjectAttachmentHandler handler;

    public AddProjectAttachmentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext1 = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext1);
        this.handler = new AddProjectAttachmentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectAttachmentAndReturnsResponse()
    {
        // Arrange
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{this.faker.System.FileName()}.pdf", this.faker.Internet.Url(), this.faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.FileName, result.FileName);
    }

    [Fact]
    public async Task Handle_VerifiesProjectAttachmentWasSaved()
    {
        // Arrange
        var fileName = $"{this.faker.System.FileName()}.pdf";
        var fileUrl = this.faker.Internet.Url();
        var fileSize = this.faker.Random.Long(1000, 1000000);
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), fileName, fileUrl, fileSize, Guid.NewGuid(), "application/json");

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var attachment = await this.db.ProjectAttachments.FirstOrDefaultAsync(a => a.Id == command.Id);

        Assert.NotNull(attachment);
        Assert.Equal(fileName, attachment.FileName);
        Assert.Equal(fileUrl, attachment.FileUrl);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectAttachments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var command1 = new AddProjectAttachmentCommand(Guid.NewGuid(), taskId, $"{this.faker.System.FileName()}.pdf", this.faker.Internet.Url(), this.faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        var command2 = new AddProjectAttachmentCommand(Guid.NewGuid(), taskId, $"{this.faker.System.FileName()}.docx", this.faker.Internet.Url(), this.faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var attachments = await this.db.ProjectAttachments.ToListAsync();
        Assert.Equal(2, attachments.Count);
    }

    [Fact]
    public async Task Handle_WithLargeFileSize_AddsProjectAttachmentSuccessfully()
    {
        // Arrange
        var largeFileSize = 1073741824L; // 1 GB
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{this.faker.System.FileName()}.zip", this.faker.Internet.Url(), largeFileSize, Guid.NewGuid(), "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(largeFileSize, result.FileSize);
    }

    [Fact]
    public async Task Handle_WithSmallFileSize_AddsProjectAttachmentSuccessfully()
    {
        // Arrange
        var smallFileSize = 100L;
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{this.faker.System.FileName()}.txt", this.faker.Internet.Url(), smallFileSize, Guid.NewGuid(), "application/json");

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(smallFileSize, result.FileSize);
    }
}
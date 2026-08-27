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
        db = new DefaultContext(options, companyContext1);
        handler = new AddProjectAttachmentHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithValidCommand_AddsProjectAttachmentAndReturnsResponse()
    {

        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{faker.System.FileName()}.pdf", faker.Internet.Url(), faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.FileName, result.FileName);
    }

    [Fact]
    public async Task Handle_VerifiesProjectAttachmentWasSaved()
    {

        var fileName = $"{faker.System.FileName()}.pdf";
        var fileUrl = faker.Internet.Url();
        var fileSize = faker.Random.Long(1000, 1000000);
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), fileName, fileUrl, fileSize, Guid.NewGuid(), "application/json");

        await handler.Handle(command, CancellationToken.None);

        var attachment = await db.ProjectAttachments.FirstOrDefaultAsync(a => a.Id == command.Id);

        Assert.NotNull(attachment);
        Assert.Equal(fileName, attachment.FileName);
        Assert.Equal(fileUrl, attachment.FileUrl);
    }

    [Fact]
    public async Task Handle_WithMultipleCommands_AddsAllProjectAttachments()
    {

        var taskId = Guid.NewGuid();
        var command1 = new AddProjectAttachmentCommand(Guid.NewGuid(), taskId, $"{faker.System.FileName()}.pdf", faker.Internet.Url(), faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        var command2 = new AddProjectAttachmentCommand(Guid.NewGuid(), taskId, $"{faker.System.FileName()}.docx", faker.Internet.Url(), faker.Random.Long(1000, 1000000), Guid.NewGuid(), "application/json");

        await handler.Handle(command1, CancellationToken.None);
        await handler.Handle(command2, CancellationToken.None);

        var attachments = await db.ProjectAttachments.ToListAsync();
        Assert.Equal(2, attachments.Count);
    }

    [Fact]
    public async Task Handle_WithLargeFileSize_AddsProjectAttachmentSuccessfully()
    {

        var largeFileSize = 1073741824L;
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{faker.System.FileName()}.zip", faker.Internet.Url(), largeFileSize, Guid.NewGuid(), "application/json");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(largeFileSize, result.FileSize);
    }

    [Fact]
    public async Task Handle_WithSmallFileSize_AddsProjectAttachmentSuccessfully()
    {

        var smallFileSize = 100L;
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), $"{faker.System.FileName()}.txt", faker.Internet.Url(), smallFileSize, Guid.NewGuid(), "application/json");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(smallFileSize, result.FileSize);
    }
}

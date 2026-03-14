using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.Attachment;

public class GetAllProjectAttachmentHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetAllProjectAttachmentHandler handler;

    public GetAllProjectAttachmentHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new GetAllProjectAttachmentHandler(this.db);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProjectAttachments_ReturnsAllProjectAttachments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var attachment1 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.db.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(attachment1.Id, result[0].Id);
        Assert.Equal(attachment2.Id, result[1].Id);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.db.ProjectAttachments.Add(attachment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.db.ProjectAttachments.Add(attachment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new AttachmentModel
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.db.ProjectAttachments.Add(attachment);
        }

        await this.db.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }
}
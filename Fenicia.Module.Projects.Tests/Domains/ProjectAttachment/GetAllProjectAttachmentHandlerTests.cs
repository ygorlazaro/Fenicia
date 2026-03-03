using Bogus;

using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

[TestFixture]
public class GetAllProjectAttachmentHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new GetAllProjectAttachmentHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private GetAllProjectAttachmentHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProjectAttachments_ReturnsAllProjectAttachments()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var attachment1 = new Common.Data.Models.ProjectAttachment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.pdf",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        var attachment2 = new Common.Data.Models.ProjectAttachment
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            FileName = $"{this.faker.System.FileName()}.docx",
            FileUrl = this.faker.Internet.Url(),
            FileSize = this.faker.Random.Long(1000, 1000000),
            UploadedBy = Guid.NewGuid(),
            ContentType = "application/json"
        };

        this.context.ProjectAttachments.AddRange(attachment1, attachment2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Id, Is.EqualTo(attachment1.Id));
            Assert.That(result[1].Id, Is.EqualTo(attachment2.Id));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new Common.Data.Models.ProjectAttachment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.context.ProjectAttachments.Add(attachment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
        {
            var attachment = new Common.Data.Models.ProjectAttachment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.context.ProjectAttachments.Add(attachment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery(10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithDefaultPagination_ReturnsFirstPageWith10Items()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        for (var i = 0; i < 25; i++)
        {
            var attachment = new Common.Data.Models.ProjectAttachment
            {
                Id = Guid.NewGuid(),
                TaskId = taskId,
                FileName = $"{this.faker.System.FileName()}_{i}.pdf",
                FileUrl = this.faker.Internet.Url(),
                FileSize = this.faker.Random.Long(1000, 1000000),
                UploadedBy = Guid.NewGuid(),
                ContentType = "application/json"
            };
            this.context.ProjectAttachments.Add(attachment);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetAllProjectAttachmentQuery();

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }
}

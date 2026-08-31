using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

public class ProjectAttachmentServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectAttachmentService _service;
    private readonly Guid _companyId;

    public ProjectAttachmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _companyId = companyContext.CompanyId;
        _service = new ProjectAttachmentService(new ProjectAttachmentRepository(_db));
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenAttachmentsExist_ReturnsListWithAttachments()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectAttachmentQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectAttachmentByIdQuery(attachment.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(attachment.Id);
        result.FileName.Should().Be(attachment.FileName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectAttachmentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesAttachment()
    {
        // Arrange
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid(), "application/pdf");

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenAttachmentExists_UpdatesAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectAttachmentCommand(attachment.Id, attachment.TaskId, _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(attachment.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.System.FileName(), _faker.Internet.Url(), _faker.Random.Long(1, 1000), Guid.NewGuid());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_SoftDeletesAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            FileName = _faker.System.FileName(),
            FileUrl = _faker.Internet.Url(),
            FileSize = _faker.Random.Long(1, 1000),
            UploadedBy = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.ProjectAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectAttachmentCommand(attachment.Id), CancellationToken.None);

        // Assert
        var deletedAttachment = await _db.ProjectAttachments.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == attachment.Id);
        deletedAttachment.Should().NotBeNull();
        deletedAttachment!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectAttachmentCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.ProjectAttachments.CountAsync();
        count.Should().Be(0);
    }
}
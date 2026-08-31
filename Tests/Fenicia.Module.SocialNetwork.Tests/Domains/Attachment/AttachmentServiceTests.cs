using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Attachment;
using Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Attachment;

public class AttachmentServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly AttachmentService _service;
    private readonly Guid _companyId;

    public AttachmentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new AttachmentService(new AttachmentRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesAttachment()
    {
        // Arrange
        var command = new AddAttachmentCommand(Guid.NewGuid(), _faker.Internet.Url(), _faker.Random.ArrayElement(new[] { "jpg", "png", "pdf", "docx" }), _faker.Random.Long(1, 1000), Guid.NewGuid());

        // Act
        var result = await _service.AddAsync(command, _companyId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.Url.Should().Be(command.Url);
        result.FileType.Should().Be(command.FileType);
        result.FileSize.Should().Be(command.FileSize);
        result.CommentId.Should().Be(command.CommentId);
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_SoftDeletesAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(new[] { "jpg", "png", "pdf", "docx" }),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteAttachmentCommand(attachment.Id), Guid.NewGuid(), CancellationToken.None);

        // Assert
        var deletedAttachment = await _db.SocialNetworkAttachments.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == attachment.Id);
        deletedAttachment.Should().NotBeNull();
        deletedAttachment!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteAttachmentCommand(Guid.NewGuid()), Guid.NewGuid(), CancellationToken.None);

        // Assert
        var count = await _db.SocialNetworkAttachments.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task GetByCommentAsync_WhenAttachmentsExist_ReturnsAttachments()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(new[] { "jpg", "png", "pdf", "docx" }),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = commentId,
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByCommentAsync(new GetAttachmentsByCommentQuery(1, 10), commentId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(attachment.Id);
        result.First().CommentId.Should().Be(commentId);
    }

    [Fact]
    public async Task GetByCommentAsync_WhenNoAttachmentsExist_ReturnsEmptyList()
    {
        // Arrange
        var commentId = Guid.NewGuid();

        // Act
        var result = await _service.GetByCommentAsync(new GetAttachmentsByCommentQuery(1, 10), commentId, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Attachment;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Attachment;

public class AttachmentRepositoryTests : IDisposable
{
    private static readonly string[] _fileTypes = ["jpg", "png", "pdf", "docx"];
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly AttachmentRepository _repository;
    private readonly Guid _companyId;

    public AttachmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new AttachmentRepository(_db);
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllAttachments()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(attachment.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(attachment.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenAttachmentIsValid_InsertsAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };

        // Act
        var result = await _repository.InsertAsync(attachment, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task DeleteAsync_WhenAttachmentExists_SoftDeletesAttachment()
    {
        // Arrange
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = Guid.NewGuid(),
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(attachment.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedAttachment = await _db.SocialNetworkAttachments.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == attachment.Id);
        deletedAttachment.Should().NotBeNull();
        deletedAttachment!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByCommentAsync_ReturnsAttachmentsByComment()
    {
        // Arrange
        var commentId = Guid.NewGuid();
        var attachment = new AttachmentModel
        {
            Id = Guid.NewGuid(),
            Url = _faker.Internet.Url(),
            FileType = _faker.Random.ArrayElement(_fileTypes),
            FileSize = _faker.Random.Long(1, 1000),
            CommentId = commentId,
            CompanyId = _companyId
        };
        _db.SocialNetworkAttachments.Add(attachment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByCommentAsync(1, 10, commentId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}

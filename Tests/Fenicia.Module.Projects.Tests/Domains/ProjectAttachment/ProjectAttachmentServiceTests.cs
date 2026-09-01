using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.DTOs;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectAttachment;

public class ProjectAttachmentServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<AttachmentModel>> _mockRepository;
    private readonly ProjectAttachmentService _service;

    public ProjectAttachmentServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IRepository<AttachmentModel>>();
        _service = new ProjectAttachmentService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenAttachmentsExist_ReturnsAttachments()
    {
        var attachments = new List<AttachmentModel>
        {
            new() { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), FileName = _faker.System.FileName(), FileUrl = _faker.Internet.Url(), FileSize = 100, UploadedBy = Guid.NewGuid(), CompanyId = Guid.NewGuid() }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<AttachmentModel>(attachments));

        var result = await _service.GetAllAsync(new GetAllProjectAttachmentQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(attachments[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentExists_ReturnsAttachment()
    {
        var attachment = new AttachmentModel { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), FileName = "x.pdf", FileUrl = "http://x", FileSize = 100, UploadedBy = Guid.NewGuid(), CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.GetByIdAsync(attachment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(attachment);

        var result = await _service.GetByIdAsync(new GetProjectAttachmentByIdQuery(attachment.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(attachment.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttachmentModel?)null);

        var result = await _service.GetByIdAsync(new GetProjectAttachmentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedAttachment()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), "x.pdf", "http://x", 100, Guid.NewGuid(), "application/pdf");

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<AttachmentModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttachmentModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenAttachmentExists_ReturnsUpdatedAttachment()
    {
        var attachment = new AttachmentModel { Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), FileName = "y.pdf", FileUrl = "http://y", FileSize = 200, UploadedBy = Guid.NewGuid(), CompanyId = Guid.NewGuid() };

        _mockRepository.Setup(r => r.UpdateAsync(attachment.Id, It.IsAny<AttachmentModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(attachment);

        var command = new UpdateProjectAttachmentCommand(attachment.Id, attachment.TaskId, "y.pdf", "http://y", 200, Guid.NewGuid());

        var result = await _service.UpdateAsync(command, attachment.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(attachment.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenAttachmentDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<AttachmentModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttachmentModel?)null);

        var command = new UpdateProjectAttachmentCommand(Guid.NewGuid(), Guid.NewGuid(), "y.pdf", "http://y", 200, Guid.NewGuid());

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectAttachmentCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
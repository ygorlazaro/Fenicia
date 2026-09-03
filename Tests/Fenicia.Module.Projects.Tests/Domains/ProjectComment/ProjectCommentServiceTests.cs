using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class ProjectCommentServiceTests
{
    private readonly Faker _faker;
    private readonly Mock<IRepository<ProjectCommentModel>> _mockRepository;
    private readonly ProjectCommentService _service;

    public ProjectCommentServiceTests()
    {
        _faker = new Faker();
        _mockRepository = new Mock<IRepository<ProjectCommentModel>>();
        _service = new ProjectCommentService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAsync_WhenCommentsExist_ReturnsComments()
    {
        var comments = new List<ProjectCommentModel>
        {
            new()
            {
                Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(),
                Content = _faker.Lorem.Sentence(), CompanyId = Guid.NewGuid()
            }
        };

        _mockRepository.Setup(r => r.Query()).Returns(new TestAsyncEnumerable<ProjectCommentModel>(comments));

        var result = await _service.GetAllAsync(new GetAllProjectCommentQuery(), CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Id.Should().Be(comments[0].Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "hello",
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var result = await _service.GetByIdAsync(new GetProjectCommentByIdQuery(comment.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(comment.Id);
        result.Content.Should().Be("hello");
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectCommentModel?)null);

        var result = await _service.GetByIdAsync(
            new GetProjectCommentByIdQuery(Guid.NewGuid()),
            CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_ReturnsCreatedComment()
    {
        var companyId = Guid.NewGuid();
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "new comment");

        _mockRepository.Setup(r => r.InsertAsync(It.IsAny<ProjectCommentModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectCommentModel m, CancellationToken _) => m);

        var result = await _service.AddAsync(command, companyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        result.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentExists_ReturnsUpdatedComment()
    {
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(), TaskId = Guid.NewGuid(), UserId = Guid.NewGuid(), Content = "updated",
            CompanyId = Guid.NewGuid()
        };

        _mockRepository.Setup(r => r.GetByIdAsync(comment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);
        _mockRepository.Setup(r => r.UpdateAsync(
                comment.Id,
                It.IsAny<ProjectCommentModel>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(comment);

        var command = new UpdateProjectCommentCommand(comment.Id, "updated");

        var result = await _service.UpdateAsync(command, comment.CompanyId, CancellationToken.None);

        result.Should().NotBeNull();
        result.Content.Should().Be("updated");
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        _mockRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProjectCommentModel?)null);

        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), "x");

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCalled_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.DeleteAsync(new DeleteProjectCommentCommand(id), CancellationToken.None);

        _mockRepository.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
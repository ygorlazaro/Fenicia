using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectComment;

public class ProjectCommentServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectCommentService _service;
    private readonly Guid _companyId;

    public ProjectCommentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ProjectCommentService(new ProjectCommentRepository(_db));
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenCommentsExist_ReturnsPaginationWithComments()
    {
        // Arrange
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllProjectCommentQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentExists_ReturnsComment()
    {
        // Arrange
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectCommentByIdQuery(comment.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(comment.Id);
        result.Content.Should().Be(comment.Content);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetProjectCommentByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesComment()
    {
        // Arrange
        var command = new AddProjectCommentCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CompanyId.Should().Be(_companyId);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentExists_UpdatesComment()
    {
        // Arrange
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectCommentCommand(comment.Id, _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(comment.Id);
        result.Content.Should().Be(command.Content);
    }

    [Fact]
    public async Task UpdateAsync_WhenCommentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProjectCommentCommand(Guid.NewGuid(), _faker.Lorem.Sentence());

        // Act
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentExists_SoftDeletesComment()
    {
        // Arrange
        var comment = new ProjectCommentModel
        {
            Id = Guid.NewGuid(),
            TaskId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Content = _faker.Lorem.Sentence(),
            CompanyId = _companyId
        };
        _db.ProjectComments.Add(comment);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteProjectCommentCommand(comment.Id), CancellationToken.None);

        // Assert
        var deletedComment = await _db.ProjectComments.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == comment.Id);
        deletedComment.Should().NotBeNull();
        deletedComment!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenCommentDoesNotExist_DoesNothing()
    {
        // Arrange

        // Act
        await _service.DeleteAsync(new DeleteProjectCommentCommand(Guid.NewGuid()), CancellationToken.None);

        // Assert
        var count = await _db.ProjectComments.CountAsync();
        count.Should().Be(0);
    }
}

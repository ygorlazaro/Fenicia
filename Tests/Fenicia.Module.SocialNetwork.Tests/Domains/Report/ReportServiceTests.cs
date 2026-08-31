using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Enums.SocialNetwork;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Report;
using Fenicia.Module.SocialNetwork.Domains.Report.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Report;

public class ReportServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ReportService _service;

    public ReportServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _service = new ReportService(new ReportRepository(_db));
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesReport()
    {
        // Arrange
        var command = new AddReportCommand(Guid.NewGuid(), Guid.NewGuid(), "Feed", _faker.Lorem.Sentence(), _faker.Lorem.Sentence());

        // Act
        var result = await _service.AddAsync(command, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TargetId.Should().Be(command.TargetId);
        result.TargetType.Should().Be(command.TargetType);
        result.Reason.Should().Be(command.Reason);
        result.Description.Should().Be(command.Description);
        result.Status.Should().Be(EnumReportStatus.Pending.ToString());
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenReportExists_UpdatesStatus()
    {
        // Arrange
        var report = new ReportModel
        {
            Id = Guid.NewGuid(),
            ReporterId = Guid.NewGuid(),
            TargetId = Guid.NewGuid(),
            TargetType = "Feed",
            Reason = _faker.Lorem.Sentence(),
            Status = EnumReportStatus.Pending,
            ReportDate = DateTime.UtcNow
        };
        _db.SocialNetworkReports.Add(report);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateReportStatusCommand(report.Id, "Approved");

        // Act
        var result = await _service.UpdateStatusAsync(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(report.Id);
        result.Status.Should().Be("Approved");
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenReportDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateReportStatusCommand(Guid.NewGuid(), "Approved");

        // Act
        var result = await _service.UpdateStatusAsync(command, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateStatusAsync_WhenStatusIsInvalid_ThrowsArgumentException()
    {
        // Arrange
        var report = new ReportModel
        {
            Id = Guid.NewGuid(),
            ReporterId = Guid.NewGuid(),
            TargetId = Guid.NewGuid(),
            TargetType = "Feed",
            Reason = _faker.Lorem.Sentence(),
            Status = EnumReportStatus.Pending,
            ReportDate = DateTime.UtcNow
        };
        _db.SocialNetworkReports.Add(report);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateReportStatusCommand(report.Id, "Pending");

        // Act
        Func<Task> act = async () => await _service.UpdateStatusAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task GetAllAsync_WhenReportsExist_ReturnsPaginationWithReports()
    {
        // Arrange
        var report = new ReportModel
        {
            Id = Guid.NewGuid(),
            ReporterId = Guid.NewGuid(),
            TargetId = Guid.NewGuid(),
            TargetType = "Feed",
            Reason = _faker.Lorem.Sentence(),
            Status = EnumReportStatus.Pending,
            ReportDate = DateTime.UtcNow
        };
        _db.SocialNetworkReports.Add(report);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllReportQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Id.Should().Be(report.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenReportExists_ReturnsReport()
    {
        // Arrange
        var report = new ReportModel
        {
            Id = Guid.NewGuid(),
            ReporterId = Guid.NewGuid(),
            TargetId = Guid.NewGuid(),
            TargetType = "Feed",
            Reason = _faker.Lorem.Sentence(),
            Status = EnumReportStatus.Pending,
            ReportDate = DateTime.UtcNow
        };
        _db.SocialNetworkReports.Add(report);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetReportByIdQuery(report.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(report.Id);
        result.TargetType.Should().Be("Feed");
    }

    [Fact]
    public async Task GetByIdAsync_WhenReportDoesNotExist_ReturnsNull()
    {
        // Arrange

        // Act
        var result = await _service.GetByIdAsync(new GetReportByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Enums.SocialNetwork;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Report;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Report;

public class ReportRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ReportRepository _repository;

    public ReportRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new ReportRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllReports()
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
        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
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
        var result = await _repository.GetByIdAsync(report.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(report.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenReportDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenReportIsValid_InsertsReport()
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

        // Act
        var result = await _repository.InsertAsync(report, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default(DateTime));
    }

    [Fact]
    public async Task UpdateAsync_WhenReportExists_UpdatesReport()
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

        report.Status = EnumReportStatus.Approved;

        // Act
        var result = await _repository.UpdateAsync(report.Id, report, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(EnumReportStatus.Approved);
    }

    [Fact]
    public async Task UpdateAsync_WhenReportDoesNotExist_ReturnsNull()
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

        // Act
        var result = await _repository.UpdateAsync(report.Id, report, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenReportExists_SoftDeletesReport()
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
        var result = await _repository.DeleteAsync(report.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedReport = await _db.SocialNetworkReports.IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id == report.Id);
        deletedReport.Should().NotBeNull();
        deletedReport!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
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
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingReports()
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
        var result = await _repository.FindAsync(r => r.Id == report.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}

using System.Security.Claims;

using AwesomeAssertions;
using Bogus;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Enums.SocialNetwork;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Report;
using Fenicia.Module.SocialNetwork.Domains.Report.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Report;

public class ReportControllerTests : IDisposable
{
    private readonly ReportController _controller;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Guid _testUserId;

    public ReportControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ReportRepository(_db);
        var service = new ReportService(repository);
        _mockHttpContext = new Mock<HttpContext>();
        _controller = new ReportController(service) { ControllerContext = new ControllerContext { HttpContext = _mockHttpContext.Object } };
        _testUserId = Guid.NewGuid();
        SetupAdminClaims(_testUserId);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task PostAsync_WhenCommandIsValid_ReturnsCreated()
    {
        // Arrange
        var command = new AddReportCommand(Guid.NewGuid(), Guid.NewGuid(), "Feed", _faker.Lorem.Sentence(), _faker.Lorem.Sentence());
        var wide = new WideEventContext();

        // Act
        var result = await _controller.PostAsync(command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<CreatedResult>();
        var createdResult = (CreatedResult)result.Result;
        var returnedReport = (AddReportResponse)createdResult.Value!;
        returnedReport.TargetId.Should().Be(command.TargetId);
        returnedReport.TargetType.Should().Be(command.TargetType);
        returnedReport.Status.Should().Be(EnumReportStatus.Pending.ToString());
    }

    [Fact]
    public async Task PatchStatusAsync_WhenReportExists_ReturnsOkWithUpdatedStatus()
    {
        // Arrange
        var wide = new WideEventContext();
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
        var result = await _controller.PatchStatusAsync(report.Id, command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedReport = (UpdateReportResponse)okResult.Value!;
        returnedReport.Id.Should().Be(report.Id);
        returnedReport.Status.Should().Be("Approved");
    }

    [Fact]
    public async Task PatchStatusAsync_WhenReportDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();
        var command = new UpdateReportStatusCommand(Guid.NewGuid(), "Approved");

        // Act
        var result = await _controller.PatchStatusAsync(Guid.NewGuid(), command, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllAsync_WhenReportsExist_ReturnsOkWithReports()
    {
        // Arrange
        var wide = new WideEventContext();
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
        var result = await _controller.GetAllAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var reports = (List<GetAllReportResponse>)okResult.Value!;
        reports.Should().HaveCount(1);
        reports.First().Id.Should().Be(report.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoReportsExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetAllAsync(wide, 1, 10, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var reports = (List<GetAllReportResponse>)okResult.Value!;
        reports.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenReportExists_ReturnsOkWithReport()
    {
        // Arrange
        var wide = new WideEventContext();
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
        var result = await _controller.GetByIdAsync(report.Id, wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result.Result;
        var returnedReport = (GetReportByIdResponse)okResult.Value!;
        returnedReport.Id.Should().Be(report.Id);
        returnedReport.TargetType.Should().Be("Feed");
    }

    [Fact]
    public async Task GetByIdAsync_WhenReportDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var wide = new WideEventContext();

        // Act
        var result = await _controller.GetByIdAsync(Guid.NewGuid(), wide, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    private void SetupAdminClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("userId", userId.ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        _mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        _controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}

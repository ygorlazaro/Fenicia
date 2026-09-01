using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class GetProjectStatusByIdServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectStatusService _service;
    private readonly Guid _companyId;

    public GetProjectStatusByIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProjectStatusRepository(_db);
        _service = new ProjectStatusService(repository);
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusExists_ReturnsStatus()
    {
        // Arrange
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), Color = "#FF0000", CompanyId = _companyId };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetProjectStatusByIdQuery(status.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(status.Name);
    }
}

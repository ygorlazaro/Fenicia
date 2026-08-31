using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class UpdateProjectStatusServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectStatusService _service;
    private readonly ProjectStatusRepository _repository;
    private readonly Guid _companyId;

    public UpdateProjectStatusServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new ProjectStatusRepository(_db);
        _service = new ProjectStatusService(_repository);
        _faker = new Faker();
        _companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task UpdateAsync_WhenStatusExists_ReturnsUpdatedStatus()
    {
        // Arrange
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1).First(), Color = "#FF0000", CompanyId = _companyId };
        _db.ProjectStatuses.Add(status);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var command = new UpdateProjectStatusCommand(status.Id, Guid.NewGuid(), _faker.Commerce.Categories(1).First(), "#00FF00", 2, true);
        var result = await _service.UpdateAsync(command, _companyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(command.Name);
    }
}

using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class AddProjectStatusServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProjectStatusService _service;
    private readonly ProjectStatusRepository _repository;
    private readonly Guid _companyId;

    public AddProjectStatusServiceTests()
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
    public async Task AddAsync_WhenValid_ReturnsCreatedStatus()
    {
        var command = new AddProjectStatusCommand(Guid.NewGuid(), Guid.NewGuid(), _faker.Commerce.Categories(1).First(), "#FF0000", 1, false);

        var result = await _service.AddAsync(command, _companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(_companyId, result.CompanyId);
    }
}

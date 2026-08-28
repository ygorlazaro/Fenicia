using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectStatus;

public class GetProjectStatusByIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProjectStatusService service;
    private readonly ProjectStatusRepository repository;
    private readonly Guid companyId;

    public GetProjectStatusByIdServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        repository = new ProjectStatusRepository(db);
        service = new ProjectStatusService(repository);
        faker = new Faker();
        companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenStatusExists_ReturnsStatus()
    {
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First(), Color = "#FF0000", CompanyId = companyId };
        db.ProjectStatuses.Add(status);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByIdAsync(new GetProjectStatusByIdQuery(status.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(status.Name, result.Name);
    }
}

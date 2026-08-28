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
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProjectStatusService service;
    private readonly ProjectStatusRepository repository;
    private readonly Guid companyId;

    public UpdateProjectStatusServiceTests()
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
    public async Task UpdateAsync_WhenStatusExists_ReturnsUpdatedStatus()
    {
        var status = new ProjectStatusModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First(), Color = "#FF0000", CompanyId = companyId };
        db.ProjectStatuses.Add(status);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProjectStatusCommand(status.Id, Guid.NewGuid(), faker.Commerce.Categories(1).First(), "#00FF00", 2, true);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Name, result.Name);
    }
}

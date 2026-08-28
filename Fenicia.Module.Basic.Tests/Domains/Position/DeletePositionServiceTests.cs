using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.Position;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Equal(0, count);
        Assert.NotNull(updated);
        Assert.NotNull(updated.Deleted);
        await db.SaveChangesAsync(CancellationToken.None);
        await service.DeleteAsync(new DeletePositionCommand(Guid.NewGuid()), companyId, CancellationToken.None);
        await service.DeleteAsync(new DeletePositionCommand(position.Id), companyId, CancellationToken.None);
        db.BasicPositions.Add(position);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Position;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly PositionService service;
    public async Task DeleteAsync_WhenPositionDoesNotExist_DoesNothing()
    public async Task DeleteAsync_WhenPositionExists_SetsDeletedDate()
public class DeletePositionServiceTests : IDisposable
    public DeletePositionServiceTests()
    public void Dispose()
        service = new PositionService(positionRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var count = await db.BasicPositions.CountAsync();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var updated = await db.BasicPositions.FindAsync(position.Id);

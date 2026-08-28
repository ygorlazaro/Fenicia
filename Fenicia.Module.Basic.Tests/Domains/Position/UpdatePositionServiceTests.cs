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
        Assert.Equal(newName, result.Name);
        Assert.Equal(position.Id, result.Id);
        Assert.NotNull(result);
        Assert.Null(result);
        await db.SaveChangesAsync(CancellationToken.None);
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
    public async Task UpdateAsync_WhenPositionDoesNotExist_ReturnsNull()
    public async Task UpdateAsync_WhenPositionExists_ReturnsUpdateResponse()
public class UpdatePositionServiceTests : IDisposable
    public UpdatePositionServiceTests()
    public void Dispose()
        service = new PositionService(positionRepository);
        var command = new UpdatePositionCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());
        var command = new UpdatePositionCommand(position.Id, newName);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var newName = faker.Commerce.Categories(1).First();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var result = await service.UpdateAsync(command, companyId, CancellationToken.None);

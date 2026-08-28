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
        Assert.Equal(position.Id, result.Id);
        Assert.Equal(position.Name, result.Name);
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
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
public class GetPositionByIdServiceTests : IDisposable
    public GetPositionByIdServiceTests()
    public void Dispose()
        service = new PositionService(positionRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var result = await service.GetByIdAsync(new GetPositionByIdQuery(Guid.NewGuid()), CancellationToken.None);
        var result = await service.GetByIdAsync(new GetPositionByIdQuery(position.Id), CancellationToken.None);

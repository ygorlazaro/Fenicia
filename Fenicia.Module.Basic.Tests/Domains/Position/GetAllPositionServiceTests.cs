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
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
        Assert.Equal(1, result.Total);
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
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
    public async Task GetAllAsync_WhenNoPositions_ReturnsEmptyPagination()
    public async Task GetAllAsync_WhenPositionsExist_ReturnsPaginationWithPositions()
public class GetAllPositionServiceTests : IDisposable
    public GetAllPositionServiceTests()
    public void Dispose()
        service = new PositionService(positionRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var result = await service.GetAllAsync(new GetAllPositionQuery(1, 10), CancellationToken.None);

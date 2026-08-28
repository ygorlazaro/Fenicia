using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class GetAllPositionServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly PositionService service;

    public GetAllPositionServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        service = new PositionService(positionRepository);
        faker = new Faker();
        var companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoPositions_ReturnsEmptyPagination()
    {
        var result = await service.GetAllAsync(new GetAllPositionQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.Total);
    }

    [Fact]
    public async Task GetAllAsync_WhenPositionsExist_ReturnsPaginationWithPositions()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAllAsync(new GetAllPositionQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(1, result.Total);
    }
}

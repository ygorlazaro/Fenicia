using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class GetPositionByIdServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly PositionService service;

    public GetPositionByIdServiceTests()
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
    public async Task GetByIdAsync_WhenPositionExists_ReturnsPosition()
    {
        var position = new PositionModel { Id = Guid.NewGuid(), Name = faker.Commerce.Categories(1).First() };
        db.BasicPositions.Add(position);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetByIdAsync(new GetPositionByIdQuery(position.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(position.Id, result.Id);
        Assert.Equal(position.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPositionDoesNotExist_ReturnsNull()
    {
        var result = await service.GetByIdAsync(new GetPositionByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }
}

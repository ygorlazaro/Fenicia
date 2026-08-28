using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Microsoft.EntityFrameworkCore;
using Fenicia.Module.Basic.Domains.Employee;

namespace Fenicia.Module.Basic.Tests.Domains.Position;

public class AddPositionServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private Guid companyId;
    private readonly PositionService service;

    public AddPositionServiceTests()
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
    public async Task AddAsync_WithValidCommand_ReturnsAddPositionResponse()
    {
        var command = new AddPositionCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());

        var result = await service.AddAsync(command, companyId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
    }
}

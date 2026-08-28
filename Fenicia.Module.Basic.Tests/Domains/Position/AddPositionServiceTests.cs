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
        Assert.Equal(command.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.NotNull(result);
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
    public AddPositionServiceTests()
    public async Task AddAsync_WithValidCommand_ReturnsAddPositionResponse()
public class AddPositionServiceTests : IDisposable
    public void Dispose()
        service = new PositionService(positionRepository);
        var command = new AddPositionCommand(Guid.NewGuid(), faker.Commerce.Categories(1).First());
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var positionRepository = new Fenicia.Module.Basic.Domains.Employee.PositionRepository(db);
        var result = await service.AddAsync(command, companyId, CancellationToken.None);

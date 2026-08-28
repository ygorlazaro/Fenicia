using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.Empty(result);
        Assert.NotNull(result);
        Assert.Single(result);
        await db.SaveChangesAsync(CancellationToken.None);
        db.AuthStates.Add(state);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.State;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StateService service;
    public async Task GetAllAsync_WhenNoStates_ReturnsEmptyList()
    public async Task GetAllAsync_WhenStatesExist_ReturnsStates()
public class GetAllStateServiceTests : IDisposable
    public GetAllStateServiceTests()
    public void Dispose()
        service = new StateService(stateRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await service.GetAllAsync(CancellationToken.None);
        var state = new StateModel { Id = Guid.NewGuid(), Name = "Sao Paulo", Uf = "SP" };
        var stateRepository = new StateRepository(db);

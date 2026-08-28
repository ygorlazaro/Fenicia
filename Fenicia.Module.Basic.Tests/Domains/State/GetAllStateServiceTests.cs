using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.State;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.State;

public class GetAllStateServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StateService service;

    public GetAllStateServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new StateService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoStates_ReturnsEmptyList()
    {
        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WhenStatesExist_ReturnsStates()
    {
        var state = new StateModel { Id = Guid.NewGuid(), Name = "Sao Paulo", Uf = "SP" };
        db.AuthStates.Add(state);
        await db.SaveChangesAsync(CancellationToken.None);

        var result = await service.GetAllAsync(CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
    }
}
